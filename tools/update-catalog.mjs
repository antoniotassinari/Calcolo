import fs from 'node:fs';
import path from 'node:path';
const walk=dir=>fs.readdirSync(dir,{withFileTypes:true}).flatMap(d=>['bin','obj'].includes(d.name)?[]:d.isDirectory()?walk(path.join(dir,d.name)):[path.join(dir,d.name)]);
const normalize=p=>p.replaceAll('\\','/');
const projects=['src','apps','tests','legacy'].flatMap(walk).filter(p=>p.endsWith('.csproj')).sort();
const rows=projects.map(project=>{const xml=fs.readFileSync(project,'utf8');return {id:path.basename(project,'.csproj'),project:normalize(project),dependencies:[...xml.matchAll(/<ProjectReference\s+Include="([^"]+)"/g)].map(m=>path.basename(m[1].replaceAll('\\','/'),'.csproj'))};});
fs.writeFileSync('modules.json',JSON.stringify({schemaVersion:1,modules:rows.filter(r=>r.project.startsWith('src/'))},null,2)+'\n');
fs.writeFileSync('docs/PROJECTS.md','# Inventario progetti\n\nGenerato da `node tools/update-catalog.mjs`. La compilazione principale comprende src/apps/tests; legacy conserva le interfacce e applicazioni storiche. Le dipendenze elencate sono riferimenti interni, non un giudizio di correttezza delle formule.\n\n| Progetto | Percorso | Dipendenze interne |\n| --- | --- | --- |\n'+rows.map(r=>`| ${r.id} | ${r.project} | ${r.dependencies.join(', ')||'—'} |`).join('\n')+'\n');
