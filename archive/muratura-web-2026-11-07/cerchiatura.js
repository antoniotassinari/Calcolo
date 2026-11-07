'use strict';
const framePanel=document.createElement('section');framePanel.id='framePanel';framePanel.className='panel';document.querySelector('.states').after(framePanel);
function frameRows(r){return [['V* = |ΔV muratura|',r.requiredV,'kN'],['W* (formula Excel per 2 colonne)',r.Wstar===null?'Non richiesto: ΔV > 0':fmt(r.Wstar),'cm³'],['Altezza profilo',r.p.h/10,'cm'],['Jx profilo - asse forte',r.p.Jx,'cm⁴'],['Wx profilo - elastico, asse forte',r.p.Wx,'cm³'],['J colonna',r.J,'cm⁴'],['W colonna',r.W,'cm³'],['Resistenza massima telaio V',r.V,'kN'],['Rigidezza telaio K',r.K,'N/cm'],['Spostamento dy telaio',r.dy,'mm'],['du minimo muratura dopo',r.availableDu,'mm'],['du muratura - dy telaio',r.duMargin,'mm'],['Resistenza effettiva mobilitata',r.Veff,'kN']]}
function frameTable(rows){return '<table><tbody>'+rows.map(([k,v,u])=>`<tr><td>${k}</td><td>${typeof v==='number'?fmt(v):esc(v)}</td><td>${u}</td></tr>`).join('')+'</tbody></table>'}
function frameComparison(r,a,b){return `<table><thead><tr><th>Confronto Excel</th><th>Prima</th><th>Dopo: muratura</th><th>Contributo telaio</th><th>Finale</th><th>Δ finale/prima</th></tr></thead><tbody><tr><td>Resistenza [kN]</td><td>${fmt(a.Vt)}</td><td>${fmt(b.Vt)}</td><td>${fmt(r.Veff)}</td><td>${fmt(r.finalV)}</td><td>${r.dV===null?'Non definita':fmt(r.dV)+' %'}</td></tr><tr><td>Rigidezza [N/cm]</td><td>${fmt(a.K)}</td><td>${fmt(b.K)}</td><td>${fmt(r.K)}</td><td>${fmt(r.finalK)}</td><td>${r.dK===null?'Non definita':fmt(r.dK)+' %'}</td></tr></tbody></table><p>Resistenza: <b>${r.dV===null?'variazione non definita':r.dV>=0?'non diminuita':'diminuita'}</b>. Rigidezza: <b>${r.dK===null?'variazione non definita':Frame.stiffnessAccepts(r.dK)?'entro ±15%':r.dK>0?'oltre +15%':'inferiore a -15%'}</b>. Criteri numerici del foglio, non attestazione di conformità.</p>`}
const frameLimits='Modello semplificato delle sole colonne, flessione sull’asse forte: J e W si sommano linearmente per profili affiancati. Non sono verificati traverso, base, nodi, ancoraggi, instabilità, interazione N-M, taglio della sezione o classe del profilo. La luce del vano non entra nelle formule del foglio. fyk e γM0 sono dati da confermare per acciaio e spessore effettivi.';

let frameDesignRequested=false;
function frameInput(f,key,label){
 return `<label>${label}<input data-frame="${key}" type="number" min="${key.startsWith('n')?1:0.000001}" step="${key.startsWith('n')?'1':'any'}" value="${f[key]}" required></label>`;
}
function frameUI(){
 const f=state.frame||Frame.defaults();
 framePanel.innerHTML=`<h2>Cerchiatura - modello Excel</h2><label class="check"><input id="frameEnabled" type="checkbox" ${f.enabled?'checked':''}>Includi il contributo della cerchiatura nel confronto finale e nella stampa</label>
 <p>Un telaio configurabile; valori iniziali dell'esempio Excel. L'attivazione è esplicita e i progetti precedenti restano solo muratura.</p>
 <div id="frameControls" ${f.enabled?'':'hidden'}>
  <div class="frame-inputs">
   <div class="frame-input-row" data-frame-row="geometry">
    ${frameInput(f,'h','Altezza telaio [cm]')}${frameInput(f,'nColumns','Numero colonne')}${frameInput(f,'nProfiles','Profili per colonna')}
   </div>
   <div class="frame-input-row frame-profile-row" data-frame-row="profile">
    <label>Profilo colonna OPPO<select data-frame="profile">${['HEA','HEB','IPE'].map(family=>`<optgroup label="${family}">${OppoProfiles.profiles.filter(p=>p.family===family).map(p=>`<option value="${p.id}" ${p.id===f.profile?'selected':''}>${p.id}</option>`).join('')}</optgroup>`).join('')}</select></label>
    <button type="button" id="frameDesignButton" aria-controls="frameDesign" aria-expanded="${frameDesignRequested}">Progetta profilo</button>
   </div>
   <div class="frame-input-row" data-frame-row="steel">
    ${frameInput(f,'E','E acciaio [daN/cm²]')}${frameInput(f,'fy','fyk [N/mm²]')}${frameInput(f,'gamma','γM0')}
   </div>
  </div>
  <label class="frame-portal">Tipo di portale<select data-frame="portal"><option value="closed" ${f.portal==='closed'?'selected':''}>Chiuso (coefficiente K = 12)</option><option value="open" ${f.portal==='open'?'selected':''}>Aperto (coefficiente K = 3)</option></select></label>
  <div id="frameDesign" aria-live="polite" ${frameDesignRequested?'':'hidden'}></div>
  <div id="frameResults"></div>
  <details class="frame-method"><summary>Ipotesi e catalogo dei profili</summary>
   <p>${frameLimits}</p>
   <p>Nel foglio il tipo di portale modifica soltanto K: la formula di V rimane invariata anche per "Aperto". Questo comportamento è riprodotto, non validato come schema strutturale generale. W* conserva l'ipotesi di due colonne del foglio; non è una verifica. La proposta del profilo usa il confronto finale di K e Veff, non W*.</p>
   <p>Profili: <a href="https://www.oppo.it/tabelle/profilati_hea.htm">OPPO HEA</a>, <a href="https://www.oppo.it/tabelle/profilati_heb.htm">HEB</a>, <a href="https://www.oppo.it/tabelle/profilati_ipe.htm">IPE</a>. Copia locale del 06/11/2026, 66 profili. Jx [cm⁴], Wx [cm³], h [mm]. Le pagine riportano UNI 5397-78 (HEA/HEB) e UNI 5398-78 (IPE), senza revisione esplicita; OPPO indica dati orientativi potenzialmente non aggiornati. Nessuna interpolazione.</p>
  </details>
 </div>`;
 document.getElementById('frameEnabled').onchange=e=>{state.frame={...f,enabled:e.target.checked};frameUI()};
 framePanel.querySelectorAll('[data-frame]').forEach(el=>el.oninput=()=>{
  state.frame??=Frame.defaults();state.frame[el.dataset.frame]=el.type==='number'?el.valueAsNumber:el.value;frameUpdate();
 });
 document.getElementById('frameDesignButton').onclick=()=>{frameDesignRequested=true;frameUpdate()};
 frameUpdate();
}
function frameDesignUI(f,a,b){
 const panel=document.getElementById('frameDesign'),button=document.getElementById('frameDesignButton');
 panel.hidden=!frameDesignRequested;button.setAttribute('aria-expanded',String(frameDesignRequested));
 if(!frameDesignRequested){panel.innerHTML='';return}
 const design=Frame.design(f,a,b);
 panel.innerHTML=`<h3>Anteprima per famiglia</h3>
 <p class="frame-design-rule">Prima taglia nominale di ogni famiglia con <b>ΔK fra −15% e +15%</b> e <b>resistenza finale maggiore dello stato prima</b>. Geometria, acciaio e portale restano quelli impostati. Anteprime ricalcolate a ogni modifica.</p>
 <div class="frame-proposals">${design.families.map(item=>{
  const r=item.candidate;
  if(!r)return `<article class="frame-proposal no-solution" data-family="${item.family}"><h4>${item.family}</h4><strong>Nessuna soluzione</strong><p>Nessuna delle ${item.checkedCount} taglie soddisfa entrambi i criteri.</p><p>Entro ±15%: ${item.stiffnessCount}.<br>Resistenza maggiore: ${item.resistanceCount}.</p></article>`;
  return `<article class="frame-proposal" data-family="${item.family}"><h4>${item.family}</h4><strong class="frame-candidate">${esc(r.p.id)}</strong><dl>
   <div><dt>ΔK finale / prima</dt><dd>${fmt(r.dK)} %</dd></div>
   <div><dt>K finale</dt><dd>${fmt(r.finalK)} N/cm</dd></div>
   <div><dt>ΔV finale / prima</dt><dd>${fmt(r.dV)} %</dd></div>
   <div><dt>V finale</dt><dd>${fmt(r.finalV)} kN</dd></div>
   <div><dt>Veff telaio</dt><dd>${fmt(r.Veff)} kN</dd></div>
  </dl><p>${item.validCount} taglie compatibili; proposta la minore.</p>
  <button type="button" data-frame-apply="${r.p.id}" ${f.profile===r.p.id?'disabled':''}>${f.profile===r.p.id?'Profilo selezionato':'Usa '+r.p.id}</button></article>`;
 }).join('')}</div>
 <p class="frame-design-limit">Selezione preliminare secondo il modello Excel, non verifica strutturale completa. La resistenza confrontata usa Veff, limitata dallo spostamento disponibile della muratura. Restano da verificare traverso, nodi, ancoraggi e gli altri limiti del modello.</p>`;
 panel.querySelectorAll('[data-frame-apply]').forEach(el=>el.onclick=()=>{
  state.frame.profile=el.dataset.frameApply;
  framePanel.querySelector('[data-frame="profile"]').value=state.frame.profile;
  frameUpdate();
 });
}
function frameUpdate(){
 const f=state.frame;
 const notice=document.querySelector('.sticky-overview>.notice');
 if(notice)notice.textContent=f?.enabled?'Confronto muratura e contributo del telaio secondo Excel. Il riepilogo in alto resta riferito alla sola muratura; il confronto finale è nel modulo cerchiatura.':'Solo muratura: nessun contributo di cerchiatura. I risultati non costituiscono una verifica completa dell’intervento.';
 if(!f?.enabled)return;
 try{
  const a=Masonry.total(state.before),b=Masonry.total(state.after),r=Frame.calc(f,a,b);
  document.getElementById('frameResults').innerHTML=`<div class="frame-details scroll" tabindex="0" role="region" aria-label="Risultati telaio, tabella scorrevole">${frameTable(frameRows(r))}</div><h3>Confronto finale come Excel</h3><div class="scroll" tabindex="0" role="region" aria-label="Confronto finale, tabella scorrevole">${frameComparison(r,a,b)}</div>`;
  frameDesignUI(f,a,b);
 }catch(e){
  document.getElementById('frameResults').innerHTML='<p class="error" role="alert">'+esc(e.message)+' Risultati telaio sospesi.</p>';
  const designPanel=document.getElementById('frameDesign');
  designPanel.hidden=!frameDesignRequested;
  document.getElementById('frameDesignButton').setAttribute('aria-expanded',String(frameDesignRequested));
  designPanel.innerHTML=frameDesignRequested?'<p class="error" role="alert">Ricerca profili sospesa: '+esc(e.message)+'</p>':'';
 }
}

const originalRender=render,originalUpdate=update;update=function(){originalUpdate();frameUpdate()};render=function(){originalRender();frameUI()};
const originalMigrate=Masonry.migrate;Masonry.migrate=function(raw){const s=originalMigrate(raw);if(s.frame){if(typeof s.frame.enabled!=='boolean')throw Error('Stato attivazione cerchiatura non valido.');if(s.frame.enabled)Frame.calc(s.frame,Masonry.total(s.before),Masonry.total(s.after))}return s};
const originalSave=$('save').onclick;$('save').onclick=async()=>{try{Frame.calc(state.frame,Masonry.total(state.before),Masonry.total(state.after));await originalSave()}catch(e){$('message').textContent='Salvataggio sospeso: '+e.message}};
function frameReport(a,b){const f=state.frame,r=Frame.calc(f,a,b);if(!r)return '';return `<h2>CERCHIATURA E CONFRONTO FINALE</h2><p>Profilo ${esc(f.profile)} · fyk ${fmt(f.fy)} N/mm² · γM0 ${fmt(f.gamma)} · h ${fmt(f.h)} cm · E ${fmt(f.E)} daN/cm² · ${f.nProfiles} profili/colonna · ${f.nColumns} colonne · portale ${f.portal==='closed'?'chiuso':'aperto'}.</p><div class="frame-report-details">${frameTable(frameRows(r))}</div><h3>Confronto finale come Excel (F86:F94)</h3>${frameComparison(r,a,b)}<p>V = 2 × ncol × fyk × 10 × Wcol / (γM0 × h) / 100 [kN]; K = nprof × ncol × c × E × 10 × Jx / h³ [N/cm], c = 12 chiuso, 3 aperto. dy = V × 10000/K [mm]. Veff = min(V × du,min,dopo / dy; V). Vfinale = ΣVt,dopo + Veff; Kfinale = ΣK,dopo + Ktelaio.</p><p>W* = 0,5 × |ΔV| × 1000 × γM0 × h / (2 × fyk × 100) [cm³], solo se ΔV ≤ 0; formula del foglio per due colonne. Nessun controllo automatico W.</p><p>${frameLimits} Per portale aperto V è invariata rispetto al chiuso, come nel foglio: ipotesi da valutare. Nessuna verifica completa dell’intervento.</p><p>Fonte sezioni: ${esc(r.p.source)} · catalogo ${esc(f.catalogVersion)}; h in mm convertita in cm, Jx cm⁴, Wx cm³. Tabelle OPPO indicative, senza data di revisione, riferimenti UNI 5397-78/5398-78. Formule Excel CS32:CS56; le incongruenze dei controlli paralleli DA54/DB54 non sono utilizzate.</p>`}
frameUI();
