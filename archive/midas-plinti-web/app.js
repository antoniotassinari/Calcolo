import {calculate,design,number} from './engine.js';
import {MidasAPI,parseTable} from './api.js';
const $=id=>document.getElementById(id);
const fields=['B','L','h','cx','cy','nodeHeight','qRd','mu','gammaG','density','extraWeight','fck','fyk','gammaC','gammaS','alphaCC','beta','cover','diameter','spacing'];
let source=null,report=null;
const escape=s=>String(s).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const fmt=n=>Number.isFinite(n)?n.toLocaleString('it-IT',{maximumFractionDigits:3}):'non applicabile';
function status(message,error=false){$('status').textContent=message;$('status').classList.toggle('error',error);}
function invalidate(){report=null;$('results').hidden=true;status('Dati modificati: eseguire nuovamente il calcolo.');}
function params(){return {...Object.fromEntries(fields.map(k=>[k,number($(k).value,k)])),limitState:$('limitState').value};}
function rows(){
  const lines=$('reactions').value.trim().split(/\r?\n/).filter(l=>l.trim());
  if(lines.length<2)throw Error('Inserire almeno una riga di reazioni');
  const sep=lines[0].includes(';')?';':',';
  const head=lines.shift().split(sep).map(x=>x.trim());
  if(head.join(',')!=='node,case,FX,FY,FZ,MX,MY,MZ')throw Error('Intestazione CSV richiesta: node,case,FX,FY,FZ,MX,MY,MZ');
  return lines.map((line,i)=>{
    const cells=line.split(sep).map(x=>x.trim());
    if(cells.length!==8)throw Error(`Riga ${i+2}: attese 8 colonne. Usare il punto decimale e nomi senza separatori CSV.`);
    return Object.fromEntries(head.map((k,j)=>[k,k==='case'?cells[j]:number(cells[j],`${k}, riga ${i+2}`)]));
  });
}
function csv(rs){return 'node,case,FX,FY,FZ,MX,MY,MZ\n'+rs.map(r=>['node','case','FX','FY','FZ','MX','MY','MZ'].map(k=>{
  if(/[;,\r\n]/.test(String(r[k])))throw Error('Nome caso contenente separatori non supportati dal CSV');return r[k];
}).join(',')).join('\n');}
function show(results,p,input,mode,notFound=[]){
  report={version:'0.1.0',createdAt:new Date().toISOString(),scope:'Verifiche parziali; non costituisce verifica normativa completa',mode,units:{force:'kN',length:'m',pressure:'kPa',reinforcement:'mm2/m',stress:'MPa'},parameters:p,input,source,notFound,results};
  $('results').hidden=false;
  $('summary').textContent=`${new Set(results.map(r=>r.node)).size} nodi · ${results.length} sestuple · ${results.filter(r=>r.ok).length} esiti favorevoli sulle verifiche implementate`;
  $('tbody').innerHTML=results.map(r=>`<tr><td>${r.node}<br>${escape(r.case)}</td><td>${fmt(r.B)} × ${fmt(r.L)} × ${fmt(r.h)}</td><td>${fmt(r.N)}</td><td>${fmt(r.qEff)} / ${fmt(r.qMin)} / ${fmt(r.qMax)}</td><td>${fmt(r.sliding)}</td><td>${fmt(r.AsX)} / ${fmt(r.AsY)} / ${fmt(r.AsProvided)}</td><td>${fmt(Math.max(r.vEdX,r.vEdY))} / ${fmt(r.vRdc)}</td><td>${fmt(r.vp)} / ${fmt(r.vRdc)}</td><td class="${r.ok?'ok':'bad'}">${r.ok?'Favorevole (parziale)':'Non soddisfatto'}<br>Terreno: ${r.soilOK?'sì':'no'}<br>CA: ${r.concreteOK===null?'non valutato':r.concreteOK?'sì':'no'}</td></tr>`).join('');
  $('issues').innerHTML=results.flatMap(r=>r.issues.map(i=>`<p>Nodo ${r.node}, ${escape(r.case)}: ${escape(i)}</p>`)).join('')+notFound.map(n=>`<p>Nodo ${n}: nessuna base soddisfa i controlli nell’intervallo; mostrate le ultime dimensioni provate.</p>`).join('');
  const r=results[0],ratio=r.B/r.L,w=200*Math.min(ratio,1.5),h=w/ratio;
  $('drawing').innerHTML=`<svg viewBox="0 0 440 300" role="img" aria-label="Pianta indicativa del primo plinto"><rect x="${220-w/2}" y="${145-h/2}" width="${w}" height="${h}" fill="#e6f2f0" stroke="#087d75" stroke-width="2"/><rect x="${220-p.cx/r.B*w/2}" y="${145-p.cy/r.L*h/2}" width="${p.cx/r.B*w}" height="${p.cy/r.L*h}" fill="#193041"/><text x="220" y="280" text-anchor="middle" fill="#193041">Nodo ${r.node} · B=${fmt(r.B)} m · L=${fmt(r.L)} m · h=${fmt(r.h)} m</text></svg>`;
  status(notFound.length?'Ricerca terminata: alcune basi non trovate.':'Calcolo completato. Consultare gli esiti per ogni combinazione.',!!notFound.length);
}
function guarded(fn){return async()=>{try{await fn();}catch(e){report=null;$('results').hidden=true;status(e.message,true);}};}
$('calculate').onclick=guarded(()=>{const p=params(),input=rows();show(calculate(input,p),p,input,'verifica');});
$('design').onclick=guarded(()=>{const p=params(),input=rows(),groups=design(input,p,number($('maxB').value,'Lato massimo'),number($('step').value,'Incremento'));show(groups.flatMap(g=>g.results),p,input,'base quadrata',groups.filter(g=>!g.found).map(g=>g.node));});
$('fetch').onclick=guarded(async()=>{
  if(!$('analysisReady').checked)throw Error('Confermare risultati aggiornati e sestuple concomitanti');
  const nodes=[...new Set($('nodes').value.trim().split(/[\s,;]+/).map(n=>number(n,'Nodo',0,true)))];
  const cases=[...new Set($('cases').value.split(/\r?\n/).map(c=>c.trim()).filter(Boolean))];
  $('fetch').disabled=true;invalidate();status('Lettura dei nodi e delle reazioni…');
  try{const api=new MidasAPI($('base').value.trim(),$('key').value.trim());const result=await api.reactions(nodes,cases);$('reactions').value=csv(result.rows);source={kind:'MIDAS API',...result};$('source').textContent=`API GEN NX · ${result.rows.length} righe · ${result.retrievedAt}`;status('Reazioni importate in kN e m. Verificare segni e confronto con GEN NX.');}finally{$('fetch').disabled=false;}
});
$('demo').onclick=()=>{invalidate();$('reactions').value='node,case,FX,FY,FZ,MX,MY,MZ\n101,SLU01(CB),20,10,900,30,45,0\n101,SLU02(CB),-15,5,750,-25,35,0\n102,SLU01(CB),10,10,650,15,20,0';$('qRd').value=250;$('mu').value=.4;source={kind:'esempio sintetico'};$('source').textContent='Esempio sintetico, non riferito al modello attivo.';status('Esempio caricato con dati del terreno dimostrativi.');};
$('file').onchange=guarded(async()=>{
  const f=$('file').files[0];if(!f)return;if(f.size>10*1024*1024)throw Error('File oltre 10 MB');
  const text=await f.text();invalidate();
  if(f.name.endsWith('.json')){const raw=JSON.parse(text);$('reactions').value=csv(parseTable(raw));source={kind:'JSON importato; unità kN/m dichiarate dall’utente',filename:f.name,raw};}
  else{$('reactions').value=text;rows();source={kind:'CSV importato; unità kN/m dichiarate dall’utente',filename:f.name};}
  $('source').textContent=`Importato ${f.name} · verificare unità kN/m e segni.`;
});
$('export').onclick=()=>{
  if(!report)return;
  const blob=new Blob([JSON.stringify(report,(_k,v)=>typeof v==='number'&&!Number.isFinite(v)?null:v,2)],{type:'application/json'}),url=URL.createObjectURL(blob),a=document.createElement('a');
  a.href=url;a.download='STA_plinti_'+new Date().toISOString().slice(0,10)+'.json';a.click();setTimeout(()=>URL.revokeObjectURL(url),1000);
};
$('print').onclick=()=>window.print();
for(const k of [...fields,'limitState','maxB','step'])$(k).addEventListener('input',invalidate);
$('reactions').addEventListener('input',()=>{source={kind:'CSV manuale'};$('source').textContent='Dati manuali: verificare unità kN/m e concomitanza.';invalidate();});
for(const k of ['base','key','nodes','cases','analysisReady'])$(k).addEventListener('input',()=>{source=null;$('reactions').value='';$('source').textContent='Connessione o filtri cambiati: rileggere le reazioni.';invalidate();});
