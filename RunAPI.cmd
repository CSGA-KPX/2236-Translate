@echo off
cd /d deep-translator-api
set https_proxy=http://localhost:1080
set http_proxy=http://localhost:1080

bash -c "https_proxy=http://localhost:1080 ~/.local/bin/poetry run python src/main.py"

pause