FROM python:3.12-slim AS builder

WORKDIR /build

COPY requirements.txt .

RUN python -m pip install \
    --no-cache-dir \
    --target /install \
    -r requirements.txt


FROM python:3.12-slim AS runtime

WORKDIR /app

RUN useradd \
    --create-home \
    --shell /usr/sbin/nologin \
    secureforge

COPY --from=builder /install /usr/local/lib/python3.12/site-packages

COPY app ./app

RUN rm -rf \
    /usr/local/lib/python3.12/site-packages/pip* \
    /usr/local/lib/python3.12/site-packages/setuptools* \
    /usr/local/lib/python3.12/site-packages/msgpack* \
    && chown -R secureforge:secureforge /app

USER secureforge

EXPOSE 8000

CMD ["python", "-m", "uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]
