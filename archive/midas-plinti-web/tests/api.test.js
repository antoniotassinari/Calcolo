import test from 'node:test';
import assert from 'node:assert/strict';
import {MidasAPI,parseTable} from '../api.js';
const table={STA_REACTIONS:{HEAD:['Index','Node','Load','FY','MX','FZ','FX','MY','MZ'],DATA:[[1,101,'SLU01(CB)',2,3,900,1,4,0]]}};
test('Mappa colonne effettive e mantiene segni e nomi',()=>{
  assert.deepEqual(parseTable(table),[{node:101,case:'SLU01(CB)',FX:1,FY:2,FZ:900,MX:3,MY:4,MZ:0}]);
});
test('Rifiuta colonne assenti, righe incomplete e numeri non validi',()=>{
  assert.throws(()=>parseTable({T:{HEAD:['Node','FZ'],DATA:[[1,3]]}}));
  assert.throws(()=>parseTable({T:{...table.STA_REACTIONS,DATA:[[1,101]]}}));
  const bad=structuredClone(table);bad.STA_REACTIONS.DATA[0][5]='N/A';assert.throws(()=>parseTable(bad));
});
test('Richiesta documentata: REACTIONG, KN/M, ID e casi assegnati; nessuna scrittura DB',async()=>{
  const calls=[];
  const api=new MidasAPI('https://example.invalid/gen','test-key',async(url,options)=>{
    calls.push({url,options});return {ok:true,json:async()=>url.endsWith('/db/NODE')?{NODE:{101:{X:0,Y:0,Z:0}}}:table};
  });
  const out=await api.reactions([101],['SLU01(CB)']);assert.equal(out.rows.length,1);assert.equal(calls.length,2);
  assert.equal(calls[0].options.method,'GET');assert.equal(calls[1].options.method,'POST');
  const a=JSON.parse(calls[1].options.body).Argument;assert.equal(a.TABLE_TYPE,'REACTIONG');assert.deepEqual(a.UNIT,{FORCE:'KN',DIST:'M'});assert.deepEqual(a.NODE_ELEMS.KEYS,[101]);assert.equal(calls[1].options.redirect,'error');
  assert.ok(!JSON.stringify(out).includes('test-key'));
});
test('Non ignora nodi o combinazioni richiesti ma mancanti',async()=>{
  const api=new MidasAPI('https://example.invalid/gen','key',async url=>({ok:true,json:async()=>url.endsWith('/db/NODE')?{NODE:{101:{},102:{}}}:table}));
  await assert.rejects(()=>api.reactions([101,102],['SLU01(CB)']),/mancanti/);
});
test('Errori MIDAS HTTP e applicativi non producono reazioni vuote',async()=>{
  for(const response of [{ok:false,status:403},{ok:true,json:async()=>({error:{message:'failed'}})}]){
    const api=new MidasAPI('https://example.invalid/gen','key',async()=>response);await assert.rejects(()=>api.request('/db/NODE'));
  }
  assert.throws(()=>new MidasAPI('http://example.invalid','key'));
});
