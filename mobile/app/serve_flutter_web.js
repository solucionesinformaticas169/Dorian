const http = require('http');
const fs = require('fs');
const path = require('path');
const root = path.resolve(__dirname, 'build/web');
const port = 52041;
const mime = {'.html':'text/html; charset=utf-8','.js':'application/javascript; charset=utf-8','.json':'application/json; charset=utf-8','.css':'text/css; charset=utf-8','.png':'image/png','.jpg':'image/jpeg','.jpeg':'image/jpeg','.gif':'image/gif','.svg':'image/svg+xml','.wasm':'application/wasm','.ttf':'font/ttf','.otf':'font/otf','.ico':'image/x-icon','.txt':'text/plain; charset=utf-8'};
const server = http.createServer((req,res)=>{
  const reqPath = decodeURIComponent((req.url || '/').split('?')[0]);
  let filePath = path.join(root, reqPath === '/' ? '/index.html' : reqPath);
  if (!filePath.startsWith(root)) { res.writeHead(403); return res.end('Forbidden'); }
  fs.stat(filePath, (err, stat) => {
    if (!err && stat.isDirectory()) filePath = path.join(filePath, 'index.html');
    fs.readFile(filePath, (readErr, data) => {
      if (readErr) {
        fs.readFile(path.join(root, 'index.html'), (fallbackErr, fallbackData) => {
          if (fallbackErr) { res.writeHead(404); return res.end('Not found'); }
          res.writeHead(200, {'Content-Type':'text/html; charset=utf-8'});
          res.end(fallbackData);
        });
        return;
      }
      res.writeHead(200, {'Content-Type': mime[path.extname(filePath).toLowerCase()] || 'application/octet-stream'});
      res.end(data);
    });
  });
});
server.listen(port, '127.0.0.1', () => {
  console.log(`Serving Flutter web build on http://127.0.0.1:${port}`);
});
