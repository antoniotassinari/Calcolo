import fs from 'node:fs';
import {spawnSync} from 'node:child_process';
import {calculate} from '../archive/midas-plinti-web/engine.js';
const original=JSON.parse(fs.readFileSync('examples/plinti-example.json','utf8'));
fs.mkdirSync('artifacts/parity',{recursive:true});
const variants=[{}, {reaction:{FX:10,FY:20,MX:30,MY:40}}, {reaction:{FZ:-1000}}, {reaction:{MY:700}}, {reaction:{MZ:1}}, {reaction:{FX:1000}}, {reaction:{MY:100},parameters:{limitState:'SLS',qRd:120}}, {parameters:{h:1}}, {parameters:{beta:1},reaction:{MY:100}}];
for(let i=0;i<variants.length;i++){
 const v=variants[i],input={...original,parameters:{...original.parameters,...v.parameters},reactions:[{...original.reactions[0],...v.reaction}]};
 const expected=JSON.parse(JSON.stringify(calculate(input.reactions,input.parameters)));
 const file=`artifacts/parity/${i}.json`;fs.writeFileSync(file,JSON.stringify(input));
 const run=spawnSync('dotnet',['apps/STA.Calcolo.Cli/bin/Release/net10.0/STA.Calcolo.Cli.dll','plinti',file],{encoding:'utf8'});
 if(run.status!==0)throw Error(run.stderr);
 const actual=JSON.parse(run.stdout).report.results;
 function compare(a,b,p=''){
  if(typeof a==='number'&&typeof b==='number'){if(Math.abs(a-b)>1e-9*Math.max(1,Math.abs(a)))throw Error(`${p}: ${a} != ${b}`);}
  else if(a&&typeof a==='object'){if(JSON.stringify(Object.keys(a))!==JSON.stringify(Object.keys(b)))throw Error(`Keys ${p}`);for(const k of Object.keys(a))compare(a[k],b[k],p+'.'+k);}
  else if(a!==b)throw Error(`${p}: ${a} != ${b}`);
 }
 compare(expected,actual);console.log(`PASS parita prototipo/C# scenario ${i+1}`);
}
