"""Smoke test dell'interfaccia con Chromium headless, con il modulo Python websockets per il solo collaudo."""
import asyncio
import json
import socket
import time
import urllib.request
import websockets
import functools
import http.server
from pathlib import Path
import shutil
import subprocess
import tempfile
import threading

PAGE = b'''<!doctype html><html><body><pre id="result">RUNNING</pre><iframe src="/index.html"></iframe><script>
document.querySelector('iframe').onload=async function(){
 const w=this.contentWindow,d=w.document,$=id=>d.getElementById(id);
 const assert=(v,m)=>{if(!v)throw Error(m)};
 try {
  await new Promise(r=>setTimeout(r,300));
  $('demo').click();assert($('reactions').value.includes('SLU01'),'Esempio non caricato');
  $('calculate').click();await new Promise(r=>setTimeout(r,100));
  assert(!$('results').hidden,'Risultati assenti: '+$('status').textContent);
  assert($('tbody').children.length===3,'Attese tre righe');
  $('B').value=2.6;$('B').dispatchEvent(new w.Event('input'));
  assert($('results').hidden,'Risultati obsoleti ancora visibili');
  $('design').click();await new Promise(r=>setTimeout(r,100));
  assert(!$('results').hidden,'Ricerca fallita: '+$('status').textContent);
  $('qRd').value='';$('calculate').click();await new Promise(r=>setTimeout(r,100));
  assert($('results').hidden&&$('status').classList.contains('error'),'Dato terreno mancante accettato');
  document.getElementById('result').textContent='PASS: esempio, calcolo, invalidazione, ricerca, dati mancanti';
 }catch(e){document.getElementById('result').textContent='FAIL: '+e.message;}
};</script></body></html>'''


class Handler(http.server.SimpleHTTPRequestHandler):
    """Serve l'applicazione e una pagina di collaudo sullo stesso origin."""
    def do_GET(self):
        """Restituisce il piccolo harness oppure i file statici del plugin."""
        if self.path == '/__smoke__':
            self.send_response(200)
            self.send_header('Content-Type', 'text/html; charset=utf-8')
            self.end_headers()
            self.wfile.write(PAGE)
        else:
            super().do_GET()

    def log_message(self, *args):
        """Evita log di richieste durante il collaudo."""
        pass


def main():
    """Avvia server effimero e browser, poi controlla l'esito del DOM."""
    root = Path(__file__).resolve().parents[1]
    browser = shutil.which('chromium') or shutil.which('google-chrome')
    if not browser:
        raise SystemExit('Chromium non disponibile')
    handler = functools.partial(Handler, directory=str(root))
    with http.server.ThreadingHTTPServer(('127.0.0.1', 0), handler) as server:
        threading.Thread(target=server.serve_forever, daemon=True).start()
        with tempfile.TemporaryDirectory() as profile:
            with socket.socket() as sock:
                sock.bind(('127.0.0.1', 0))
                port = sock.getsockname()[1]
            process = subprocess.Popen([browser, '--headless', '--no-sandbox', '--disable-dev-shm-usage',
                '--disable-gpu', '--disable-background-networking', '--no-first-run',
                '--user-data-dir=' + profile, '--remote-debugging-port=' + str(port),
                'about:blank'], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            try:
                opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
                endpoint = None
                for _ in range(100):
                    try:
                        with opener.open(f'http://127.0.0.1:{port}/json/version', timeout=.5) as response:
                            endpoint = json.load(response)['webSocketDebuggerUrl']
                        break
                    except OSError:
                        time.sleep(.1)
                if not endpoint:
                    raise RuntimeError('Debugger Chromium non disponibile')

                async def verify():
                    """Controlla il DOM nel browser tramite Chrome DevTools Protocol."""
                    async with websockets.connect(endpoint, proxy=None) as ws:
                        sequence = 0

                        async def call(method, params=None, session=None):
                            nonlocal sequence
                            sequence += 1
                            message = {'id': sequence, 'method': method, 'params': params or {}}
                            if session:
                                message['sessionId'] = session
                            await ws.send(json.dumps(message))
                            while True:
                                result = json.loads(await asyncio.wait_for(ws.recv(), 15))
                                if result.get('id') == sequence:
                                    if 'error' in result:
                                        raise RuntimeError(result['error'])
                                    return result.get('result', {})

                        target = await call('Target.createTarget', {'url': f'http://127.0.0.1:{server.server_port}/__smoke__'})
                        attached = await call('Target.attachToTarget', {'targetId': target['targetId'], 'flatten': True})
                        session = attached['sessionId']
                        for _ in range(150):
                            result = await call('Runtime.evaluate', {'expression': 'document.getElementById("result")?.textContent', 'returnByValue': True}, session)
                            text = result.get('result', {}).get('value', '')
                            if text.startswith('PASS:'):
                                return
                            if text.startswith('FAIL:'):
                                raise RuntimeError(text)
                            await asyncio.sleep(.1)
                        raise RuntimeError('Timeout caricamento/calcolo interfaccia')

                asyncio.run(verify())
            finally:
                process.terminate()
                try:
                    process.wait(timeout=5)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait()
        server.shutdown()
        print('PASS: esempio, calcolo, invalidazione, ricerca, dati mancanti')


if __name__ == '__main__':
    main()
