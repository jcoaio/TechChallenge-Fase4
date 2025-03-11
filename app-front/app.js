// Importando o Express
const express = require("express");
const path = require("path");
const os = require("os");

const app = express();
const PORT = 3001;

// Servindo arquivos estáticos da pasta 'public'
app.use(express.static(path.join(__dirname, "public")));
const hostname = os.hostname();

// Rota principal
app.get("/", (req, res) => {
  res.sendFile(path.join(__dirname, "public", "index.html"));
});

app.get("/hostname", (req, res) => {
    res.json({ hostname });
});
  

// Iniciando o servidor
app.listen(PORT, () => {
  console.log(`Servidor rodando em http://localhost:${PORT}`);
});
