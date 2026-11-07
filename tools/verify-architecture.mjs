import fs from 'node:fs';
import path from 'node:path';
const walk=dir=>fs.readdirSync(dir,{withFileTypes:true}).flatMap(d=>d.name==='bin'||d.name==='obj'?[]:d.isDirectory()?walk(path.join(dir,d.name)):[path.join(dir,d.name)]);
const projects=['src','apps','tests','legacy'].flatMap(walk).filter(p=>p.endsWith('.csproj'));
const graph=new Map();
for(const project of projects){
 const xml=fs.readFileSync(project,'utf8');const refs=[...xml.matchAll(/<ProjectReference\s+Include="([^"]+)"/g)].map(m=>path.normalize(path.join(path.dirname(project),m[1].replaceAll('\\','/'))));
 for(const ref of refs)if(!fs.existsSync(ref))throw Error(`${project}: riferimento inesistente ${ref}`);
 if(project.startsWith('src')){
  if(/UseWPF|UseWindowsForms|net10.0-windows|<HintPath>/.test(xml))throw Error(`Dipendenza piattaforma nel core ${project}`);
  for(const ref of refs)if(!ref.startsWith('src'+path.sep))throw Error(`Dipendenza inversa ${project}: ${ref}`);
 }
 graph.set(path.normalize(project),refs);
}
function visit(p,trail=[]){if(trail.includes(p))throw Error('Ciclo: '+[...trail,p].join(' -> '));for(const ref of graph.get(p)||[])visit(ref,[...trail,p]);}
for(const project of graph.keys())visit(project);
for(const file of walk('src').filter(p=>p.endsWith('.cs'))){const text=fs.readFileSync(file,'utf8');if(/using System\.Windows|DllImport\(/.test(text))throw Error(`Core non portabile: ${file}`);}
const manifest=JSON.parse(fs.readFileSync('modules.json','utf8'));
for(const m of manifest.modules){if(!fs.existsSync(m.project))throw Error(`Modulo assente ${m.id}`);}
if(manifest.modules.length!==projects.filter(p=>p.startsWith('src'+path.sep)).length)throw Error('Catalogo incompleto');
console.log(`PASS architettura: ${projects.length} progetti, riferimenti esistenti, nessun ciclo, core senza UI o DLL native`);

for(const file of ['apps/STA.Calcolo.Desktop/ViewModels','apps/STA.Calcolo.Updater/ViewModels'].flatMap(walk).filter(p=>p.endsWith('.cs'))){
 const text=fs.readFileSync(file,'utf8');if(/using System\.Windows|Microsoft\.Win32|InitializeComponent|MessageBox/.test(text))throw Error('ViewModel dipendente dalla View: '+file);
}
for(const file of ['apps/STA.Calcolo.Desktop','apps/STA.Calcolo.Updater'].flatMap(walk).filter(p=>p.endsWith('.xaml.cs')&&!p.endsWith('App.xaml.cs'))){
 const text=fs.readFileSync(file,'utf8');if(!/public partial class \w+ : (?:Window|UserControl)\s*\{\s*public \w+\(\)\s*\{\s*InitializeComponent\(\);\s*\}\s*\}/.test(text))throw Error('Logica nel code-behind: '+file);
}
console.log('PASS MVVM: ViewModel senza dipendenze dalle finestre; code-behind solo inizializzazione');

