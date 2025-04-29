<h1>🏍️ Sistema de Aluguel de Motos e Gestão de Entregadores</h1>

<p><strong>Tecnologias principais:</strong></p>
<ul>
  <li>.NET 8</li>
  <li>PostgreSQL 15</li>
  <li>nHibernate 5.3</li>
  <li>RabbitMQ 3.12</li>
</ul>

<h2>📌 Visão Geral</h2>
<p>Sistema backend para gestão de aluguel de motos e cadastro de entregadores com:</p>
<ul>
  <li>CRUD completo de motocicletas</li>
  <li>Validação de CNH (categorias A/B)</li>
  <li>Sistema de mensageria com RabbitMQ</li>
  <li>Mapeamento ORM com nHibernate</li>
</ul>

<h2>⚙️ Configuração</h2>

<h3>Banco de Dados (PostgreSQL)</h3>
<pre>docker run --name moto-pg -e POSTGRES_PASSWORD=senha -p 5432:5432 -d postgres:15</pre>

<h3>RabbitMQ</h3>
<pre>docker run -d --hostname moto-rabbit -p 5672:5672 rabbitmq:3-management</pre>

<h3>Arquivo de Configuração</h3>
<pre>
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=moto_rental;Username=postgres;Password=senha"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "QueueName": "motorcycle_events"
  }
}
</pre>

<h2>🚀 Funcionalidades Principais</h2>
<ul>
  <li><strong>Módulo Motos:</strong>
    <ul>
      <li>Validação de placa única</li>
      <li>Eventos via RabbitMQ ao cadastrar</li>
      <li>Filtro por placa</li>
    </ul>
  </li>
  <li><strong>Módulo Entregadores:</strong>
    <ul>
      <li>Validação de CNH A/B</li>
      <li>Armazenamento de imagens da CNH</li>
      <li>Validação de CNPJ único</li>
    </ul>
  </li>
</ul>

<h2>📂 Estrutura do Projeto</h2>
<pre>
src/
├── WebApi/          # Controllers e Middleware
├── Application/     # Serviços e DTOs
├── Domain/          # Mapeamentos nHibernate
└── Infrastructure/  # RabbitMQ e PostgreSQL
</pre>

<p><strong>Repositório:</strong> <a href="https://github.com/jeffreyal91/Motorcycle-.NET">github.com/jeffreyal91/Motorcycle-.NET</a></p>
