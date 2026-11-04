import test from 'node:test';
import assert from 'node:assert/strict';
import {check,calculate,design,concurrent} from '../engine.js';
const p={B:3,L:3,h:.5,cx:.4,cy:.4,qRd:250,fck:30,fyk:450,gammaC:1.5,gammaS:1.15,gammaG:1.3,beta:1.5,cover:50,diameter:16,spacing:150,density:25,mu:.4,extraWeight:0,nodeHeight:.5,limitState:'ULS',alphaCC:.85};
const r={node:1,case:'ULS01(CB)',FX:0,FY:0,FZ:900,MX:0,MY:0,MZ:0};
const near=(a,b)=>assert.ok(Math.abs(a-b)<1e-8,`${a} != ${b}`);
test('Carico centrato: equilibrio, peso e pressione calcolabili a mano',()=>{
  const c=check(r,p);near(c.weight,112.5);near(c.N,1046.25);near(c.qEff,116.25);near(c.qMin,116.25);near(c.qMax,116.25);assert.equal(c.ok,true);
});
test('Trasporto delle reazioni: -R e prodotto vettoriale alla quota nodo',()=>{
  const c=check({...r,FX:10,FY:20,MX:30,MY:40},p);near(c.Mx,-20);near(c.My,-45);near(c.ex,45/c.N);near(c.ey,20/c.N);
});
test('Eccentricità biassiale: qmin e area efficace indipendenti',()=>{
  const c=check({...r,MX:90,MY:120},{...p,density:0});near(c.Be,3-2*120/900);near(c.Le,2.8);near(c.qMin,100-6*120/27-6*90/27);near(c.qEff,900/(c.Be*c.Le));
});
test('Trazione, risultante fuori base e contatto parziale non risultano verificati',()=>{
  for(const row of [{...r,FZ:-1000},{...r,MY:2000},{...r,MY:700}]){const c=check(row,p);assert.equal(c.ok,false);assert.ok(c.issues.length);}
});
test('Controlla torsione e scorrimento senza ignorare componenti orizzontali',()=>{
  assert.equal(check({...r,MZ:1},p).ok,false);assert.equal(check({...r,FX:1000},p).soilOK,false);
});
test('SLE usa la pressione massima e non dichiara verifiche CA',()=>{
  const c=check({...r,MY:100},{...p,limitState:'SLS',qRd:120});assert.equal(c.soilOK,false);assert.equal(c.concreteOK,null);
});
test('Rifiuta dati mancanti, non finiti e geometrie incompatibili',()=>{
  for(const patch of [{qRd:NaN},{qRd:''},{B:0},{h:.01},{cx:5},{fck:90},{spacing:0}])assert.throws(()=>check(r,{...p,...patch}));
  assert.throws(()=>check({...r,FX:''},p));
});
test('Inviluppi e doppie righe non creano sestuple artificiali',()=>{
  for(const name of ['C1(CB:max)','C1(CB:min)','C1(CB:all)','EQ(RS)','MOV(MV)',''])assert.throws(()=>concurrent(name));
  assert.throws(()=>calculate([r,r],p));assert.throws(()=>calculate([],p));
});
test('Ricerca su più nodi conserva tutti i casi e seleziona la prima base valida',()=>{
  const rows=[r,{...r,case:'ULS02(CB)',FZ:1000},{...r,node:2,FZ:800}];
  const out=design(rows,{...p,B:2,L:2},4,.1);
  assert.equal(out.length,2);assert.equal(out[0].results.length,2);assert.ok(out.every(g=>g.found));
  const B=out[0].results[0].B;assert.ok(calculate(rows.filter(r=>r.node===1),{...p,B:B-.1,L:B-.1}).some(r=>!r.ok));
});
test('Ricerca non trovata distinta da esito positivo; griglia limitata',()=>{
  assert.equal(design([{...r,FZ:100000}],p,3.2,.1)[0].found,false);
  assert.throws(()=>design([r],p,100,.001));assert.throws(()=>design([r],p,4,0));
});
test('Punzonamento: β insufficiente e perimetro esterno impediscono esito favorevole',()=>{
  assert.equal(check({...r,MX:100},{...p,beta:1}).concreteOK,false);
  assert.equal(check(r,{...p,B:2,L:2,h:.7}).concreteOK,false);
});
