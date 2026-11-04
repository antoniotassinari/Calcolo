import {number,concurrent} from './engine.js';

/** Collegamento limitato a letture DB ed estrazione risultati, senza modifiche al modello. */
export class MidasAPI {
  constructor(base,key,fetcher=fetch) {
    const u=new URL(base);
    if(u.protocol!=='https:'||u.username||u.password||u.search||u.hash) throw Error('Base URL HTTPS non valido');
    if(!key.trim()) throw Error('MAPI-Key mancante');
    this.base=base.replace(/\/+$/,'');this.key=key;this.fetcher=fetcher;
  }
  async request(path,body) {
    if(!['/db/NODE','/post/TABLE'].includes(path)) throw Error('Operazione non consentita');
    const r=await this.fetcher(this.base+path,{method:body?'POST':'GET',redirect:'error',signal:AbortSignal.timeout(60000),headers:{'Content-Type':'application/json','MAPI-Key':this.key},...(body?{body:JSON.stringify(body)}:{})});
    if(!r.ok) throw Error(`MIDAS HTTP ${r.status}: verificare connessione API e compatibilità`);
    const data=await r.json();
    if(data.error||data.ERROR) throw Error('MIDAS ha restituito un errore: controllare modello, analisi e nomi delle combinazioni');
    return data;
  }
  async reactions(nodes,cases) {
    if(!nodes.length||!cases.length) throw Error('Assegnare nodi e combinazioni');
    nodes.forEach(n=>{number(n,'Nodo',0,true);if(!Number.isInteger(n))throw Error('ID nodo non intero');});
    cases.forEach(concurrent);
    const model=await this.request('/db/NODE');
    for(const id of nodes) if(!model.NODE?.[id])throw Error(`Nodo ${id} assente nel modello`);
    const request={Argument:{TABLE_NAME:'STA_REACTIONS',TABLE_TYPE:'REACTIONG',UNIT:{FORCE:'KN',DIST:'M'},STYLES:{FORMAT:'Scientific',PLACE:12},NODE_ELEMS:{KEYS:nodes},LOAD_CASE_NAMES:cases}};
    const raw=await this.request('/post/TABLE',request);
    const rows=parseTable(raw);
    for(const r of rows) if(!nodes.includes(r.node)||!cases.includes(r.case)) throw Error('MIDAS ha restituito nodi/combinazioni diversi da quelli richiesti');
    for(const n of nodes)for(const c of cases) if(!rows.some(r=>r.node===n&&r.case===c))throw Error(`Risultati mancanti: nodo ${n}, combinazione ${c}`);
    return {rows,raw,request,coordinates:Object.fromEntries(nodes.map(n=>[n,model.NODE[n]])),retrievedAt:new Date().toISOString()};
  }
}

/** Legge le intestazioni effettive, senza indici fissi per le componenti. */
export function parseTable(raw) {
  const tables=Object.values(raw).filter(t=>t&&Array.isArray(t.HEAD)&&Array.isArray(t.DATA));
  if(tables.length!==1) throw Error('Attesa una sola tabella con HEAD e DATA');
  const t=tables[0];
  const norm=s=>String(s).toUpperCase().replace(/\([^)]*\)|\[[^\]]*\]/g,'').replace(/[^A-Z0-9]/g,'');
  const headers=t.HEAD.map(norm);
  const aliases={node:['NODE','NODENO','NODEID'],case:['LOAD','LOADCASE','LOADCASENAME','LOADCASECOMB','LOADCOMB','LOADCOMBINATION'],FX:['FX'],FY:['FY'],FZ:['FZ'],MX:['MX'],MY:['MY'],MZ:['MZ']};
  const indices=Object.fromEntries(Object.entries(aliases).map(([k,names])=>{
    const found=headers.flatMap((h,i)=>names.includes(h)?[i]:[]);
    if(found.length!==1) throw Error(`Intestazione ${k} assente o ambigua: ${t.HEAD.join(', ')}`);
    return [k,found[0]];
  }));
  if(!t.DATA.length)throw Error('Tabella reazioni vuota: verificare analisi e filtri');
  return t.DATA.map(row=>{
    if(!Array.isArray(row)||row.length!==t.HEAD.length)throw Error('Riga risultati non coerente con HEAD');
    return Object.fromEntries(Object.entries(indices).map(([k,i])=>[k,k==='case'?String(row[i]):number(row[i],k)]));
  });
}
