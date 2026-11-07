(function(root){'use strict';
const defaults=()=>({enabled:false,profile:'HEA 180',catalogVersion:'oppo-2026-11-06',fy:275,gamma:1.05,h:150,E:2100000,nProfiles:1,nColumns:2,portal:'closed'});
function calc(f,before,after){if(!f||!f.enabled)return null;for(const k of ['fy','gamma','h','E','nProfiles','nColumns'])if(!Number.isFinite(f[k])||f[k]<=0)throw Error('Cerchiatura: '+k+' deve essere positivo.');for(const k of ['nProfiles','nColumns'])if(!Number.isSafeInteger(f[k]))throw Error('Cerchiatura: numero di profili e colonne deve essere intero.');if(!['closed','open'].includes(f.portal))throw Error('Tipo di portale non valido.');if(f.catalogVersion!==root.OppoProfiles.version)throw Error('Versione catalogo OPPO non disponibile.');const p=root.OppoProfiles.profiles.find(x=>x.id===f.profile);if(!p)throw Error('Profilo OPPO non disponibile.');const J=p.Jx*f.nProfiles,W=p.Wx*f.nProfiles,V=2*f.nColumns*f.fy*10*W/(f.gamma*f.h)/100,K=f.nProfiles*f.nColumns*(f.portal==='closed'?12:3)*f.E*10*p.Jx/f.h**3,dy=V*10000/K,Veff=Math.min(V*after.du/dy,V),finalV=after.Vt+Veff,finalK=after.K+K,dV=before.Vt===0?null:(finalV/before.Vt-1)*100,dK=before.K===0?null:(finalK/before.K-1)*100,Wstar=after.Vt>before.Vt?null:.5*Math.abs(after.Vt-before.Vt)*1000*f.gamma*f.h/(2*f.fy*100);if(![J,W,V,K,dy,Veff,finalV,finalK].every(x=>Number.isFinite(x)&&x>0))throw Error('Cerchiatura: valori fuori dal campo numerico.');return {p,J,W,V,K,dy,Veff,finalV,finalK,dV,dK,Wstar,requiredV:Math.abs(after.Vt-before.Vt),availableDu:after.du,duMargin:after.du-dy}}

// Machine-roundoff guard only: the engineering band remains exactly +/-15%.
function stiffnessAccepts(delta){return Number.isFinite(delta)&&Math.abs(delta)<=15+8*Number.EPSILON*100}
const designFamilies=['HEA','HEB','IPE'];
function design(f,before,after){
 if(!f?.enabled)throw Error('Attivare la cerchiatura prima della ricerca.');
 if(!before||!Number.isFinite(before.K)||before.K<=0||!Number.isFinite(before.Vt)||before.Vt<=0)throw Error('La ricerca richiede rigidezza e resistenza dello stato prima positive.');
 calc(f,before,after);
 const families=designFamilies.map(family=>{
  const checked=root.OppoProfiles.profiles.filter(p=>p.family===family).sort((a,b)=>a.size-b.size).map(p=>{
   const result=calc({...f,profile:p.id},before,after);
   return {result,stiffnessOK:stiffnessAccepts(result.dK),resistanceOK:result.finalV>before.Vt};
  });
  const valid=checked.filter(x=>x.stiffnessOK&&x.resistanceOK);
  return {family,candidate:valid[0]?.result||null,validCount:valid.length,checkedCount:checked.length,stiffnessCount:checked.filter(x=>x.stiffnessOK).length,resistanceCount:checked.filter(x=>x.resistanceOK).length};
 });
 return {families,beforeK:before.K,beforeV:before.Vt,minDeltaK:-15,maxDeltaK:15};
}

const api={defaults,calc,design,stiffnessAccepts};root.Frame=api;if(typeof module!=='undefined')module.exports=api;
})(typeof window!=='undefined'?window:globalThis);
