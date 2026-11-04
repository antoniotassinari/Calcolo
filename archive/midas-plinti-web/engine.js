/** Calcolo in kN, m, kPa; materiali in MPa e armature in mm²/m. */
export function number(value, name, min = -Infinity, strict = false) {
  if (value === null || value === '' || typeof value === 'boolean') throw Error(`${name}: valore mancante`);
  const n = Number(value);
  if (!Number.isFinite(n) || (strict ? n <= min : n < min)) throw Error(`${name}: valore non valido`);
  return n;
}

/** Valida dati di progetto senza attribuire resistenze al terreno. */
export function validate(p) {
  for (const k of ['B','L','h','cx','cy','qRd','fck','fyk','gammaC','gammaS','gammaG','beta']) number(p[k], k, 0, true);
  for (const k of ['cover','diameter','spacing','density','mu','extraWeight','nodeHeight']) number(p[k],k,0);
  if (p.diameter <= 0 || p.spacing <= 0 || p.beta < 1 || p.gammaC < 1 || p.gammaS < 1) throw Error('Diametri, passi o coefficienti non validi');
  if (p.cx >= p.B || p.cy >= p.L) throw Error('Il pilastro deve essere interno al plinto');
  if (p.h - p.cover / 1000 - 1.5*p.diameter/1000 <= 0) throw Error('Altezza utile insufficiente');
  if (!['ULS','SLS'].includes(p.limitState)) throw Error('Selezionare SLU o SLE');
  if (p.alphaCC <= 0 || p.alphaCC > 1 || !Number.isFinite(p.alphaCC)) throw Error('alphaCC non valido');
  if (p.fck<12 || p.fck>50 || p.fyk<400 || p.fyk>600) throw Error('Campo implementato: fck 12–50 MPa, fyk 400–600 MPa');
  return p;
}

/** Non si combinano estremi indipendenti o risultati spettrali non concomitanti. */
export function concurrent(name) {
  if (!String(name).trim() || /(?:CB\s*:\s*(?:max|min|all)|envelope|invilupp|\b(?:max|min)\b|\(RS\)|\(MV\)|\(SM\)|\(TH\))/i.test(name))
    throw Error(`Combinazione non concomitante o non supportata: ${name}`);
}

/** Equilibrio e verifiche per una sestupla concomitante, pilastro centrato. */
export function check(reaction, params) {
  const p=validate(params); concurrent(reaction.case);
  const r=Object.fromEntries(['FX','FY','FZ','MX','MY','MZ'].map(k=>[k,number(reaction[k],k)]));
  // Le reazioni sono azioni del vincolo sulla struttura. Sul plinto agisce -R.
  const weight=p.B*p.L*p.h*p.density + p.extraWeight;
  const N=r.FZ + p.gammaG*weight;
  const Mx=-r.MX+p.nodeHeight*r.FY, My=-r.MY-p.nodeHeight*r.FX;
  const H=Math.hypot(r.FX,r.FY), ex=N>0?Math.abs(My/N):Infinity, ey=N>0?Math.abs(Mx/N):Infinity;
  const Be=p.B-2*ex, Le=p.L-2*ey, area=Be>0&&Le>0?Be*Le:0;
  const qEff=N>0&&area>0?N/area:Infinity;
  const qMean=N/(p.B*p.L), delta=6*Math.abs(My)/(p.L*p.B**2)+6*Math.abs(Mx)/(p.B*p.L**2);
  const qMax=qMean+delta,qMin=qMean-delta;
  const fullContact=N>0&&qMin>=-1e-8, sliding=N>0 ? H/(p.mu*N || 1e-100):Infinity;
  const issues=[];
  if(N<=0) issues.push('Sollevamento: risultante verticale non compressiva');
  if(!area) issues.push('Risultante esterna alla base efficace');
  if(!fullContact) issues.push('Contatto parziale: distribuzione elastica e verifiche CA non applicabili');
  if(Math.abs(r.MZ)>1e-6) issues.push('Torsione MZ: verifica non implementata');
  const soilOK=N>0&&area>0&&(p.limitState==='SLS'?qMax:qEff)<=p.qRd&&fullContact&&sliding<=1&&Math.abs(r.MZ)<=1e-6;
  const d=p.h-p.cover/1000-1.5*p.diameter/1000;
  const fyd=p.fyk/p.gammaS, fcd=p.alphaCC*p.fck/p.gammaC;
  const AsProvided=1000/p.spacing*Math.PI*p.diameter**2/4;
  const fctm=0.3*p.fck**(2/3), AsMin=Math.max(.26*fctm/p.fyk,.0013)*1000*d*1000;
  // Pressione massima lorda, uniforme: maggiorazione conservativa dei carichi verso l'alto.
  const ax=(p.B-p.cx)/2,ay=(p.L-p.cy)/2;
  const mX=Math.max(0,qMax)*ax**2/2,mY=Math.max(0,qMax)*ay**2/2;
  const AsX=Math.max(AsMin,mX*1e6/(.9*d*1000*fyd));
  const AsY=Math.max(AsMin,mY*1e6/(.9*d*1000*fyd));
  const rho=Math.min(AsProvided/(1000*d*1000),.02),k=Math.min(1+Math.sqrt(200/(d*1000)),2);
  const vRdc=Math.max(.18/p.gammaC*k*(100*rho*p.fck)**(1/3),.035*k**1.5*Math.sqrt(p.fck));
  const vEdX=Math.max(0,qMax)*Math.max(0,ax-d)/(d*1000),vEdY=Math.max(0,qMax)*Math.max(0,ay-d)/(d*1000);
  // Perimetro a 2d interamente interno: geometria con angoli arrotondati EC2.
  const internalPerimeter=ax>=2*d&&ay>=2*d;
  const u1=2*(p.cx+p.cy)+4*Math.PI*d;
  const Ainside=p.cx*p.cy+4*d*(p.cx+p.cy)+4*Math.PI*d*d;
  // Si sottrae solo la pressione minima: stima conservativa della reazione interna.
  const Vp=Math.max(0,N-Math.max(0,qMin)*Ainside);
  const vp=p.beta*Vp/(u1*d*1000);
  // EC2 6.4.3(6), stima per pilastro interno rettangolare con eccentricità biassiale.
  const betaMin=N>0?1+1.8*Math.hypot(My/(N*p.cx),Mx/(N*p.cy)):Infinity;
  const vFace=p.beta*N/(2*(p.cx+p.cy)*d*1000);
  const vMax=.5*.6*(1-p.fck/250)*fcd;
  const flexLimit=.167*fcd*1000*(d*1000)**2/1e6;
  if(!internalPerimeter) issues.push('Perimetro di punzonamento a 2d oltre il bordo: serve verifica specifica');
  if(p.beta<betaMin) issues.push('β assegnato inferiore alla stima per eccentricità biassiale del pilastro interno');
  if(Math.max(mX,mY)>flexLimit) issues.push('Flessione oltre il campo del braccio interno semplificato z=0,9d');
  const spacingOK=p.spacing<=Math.min(3*p.h*1000,400);
  if(!spacingOK) issues.push('Passo armatura oltre il limite adottato min(3h, 400 mm)');
  const concreteOK=fullContact&&internalPerimeter&&p.beta>=betaMin&&AsProvided>=AsX&&AsProvided>=AsY&&vEdX<=vRdc&&vEdY<=vRdc&&vp<=vRdc&&vFace<=vMax&&Math.max(mX,mY)<=flexLimit&&spacingOK;
  if(p.limitState==='SLS') issues.push('SLE: verificati solo equilibrio e pressione; cedimenti e fessurazione non calcolati');
  return {node:reaction.node,case:reaction.case,B:p.B,L:p.L,h:p.h,N,H,Mx,My,weight,ex,ey,Be,Le,qEff,qMin,qMax,sliding,d,AsProvided,AsMin,AsX,AsY,mX,mY,vEdX,vEdY,vRdc,vp,vFace,vMax,betaMin,soilOK,concreteOK:p.limitState==='ULS'?concreteOK:null,
    ok:soilOK&&(p.limitState==='SLS'||concreteOK),issues};
}

/** Verifica ogni riga, senza creare una sestupla da massimi indipendenti. */
export function calculate(rows,p) {
  if(!rows.length) throw Error('Nessuna reazione assegnata');
  const seen=new Set();
  return rows.map(r=>{
    const node=number(r.node,'Nodo',0,true);
    if(!Number.isInteger(node)) throw Error('ID nodo non intero');
    const id=`${node}|${r.case}`;
    if(seen.has(id)) throw Error(`Righe duplicate per nodo/combinazione: ${id}`);
    seen.add(id);return check({...r,node},p);
  });
}

/** Cerca la base quadrata minima su griglia; altezza e armatura restano assegnate. */
export function design(rows,p,maxB,step) {
  validate(p);number(maxB,'Lato massimo',p.B);number(step,'Incremento',0,true);
  if(Math.ceil((maxB-p.B)/step)>500) throw Error('Ricerca limitata a 500 incrementi');
  const groups=new Map(),out=[];
  for(const r of rows){if(!groups.has(r.node))groups.set(r.node,[]);groups.get(r.node).push(r);}
  for(const [node,rs] of groups) {
    let found=null,last=null;
    for(let i=0;p.B+i*step<=maxB+1e-9;i++) {
      const B=Number((p.B+i*step).toFixed(8));
      last=calculate(rs,{...p,B,L:B});
      if(last.every(r=>r.ok)){found=last;break;}
    }
    out.push({node,found:!!found,results:found||last});
  }
  return out;
}
