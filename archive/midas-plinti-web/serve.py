"""Avvia il plugin web soltanto su localhost, senza dipendenze aggiuntive."""
import functools
import http.server
from pathlib import Path
import threading
import webbrowser


def main():
    """Serve i file del plugin e apre il browser predefinito."""
    directory = str(Path(__file__).resolve().parent)
    handler = functools.partial(http.server.SimpleHTTPRequestHandler, directory=directory)
    with http.server.ThreadingHTTPServer(('127.0.0.1', 8765), handler) as server:
        url = 'http://127.0.0.1:8765'
        print('STA Plinti per MIDAS GEN NX: ' + url)
        print('Premere Ctrl+C per terminare.')
        threading.Timer(0.5, lambda: webbrowser.open(url)).start()
        try:
            server.serve_forever()
        except KeyboardInterrupt:
            pass


if __name__ == '__main__':
    main()
