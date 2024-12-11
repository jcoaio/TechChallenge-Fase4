# TechChallenge-Fase3

## O problema
Continuando a evolução do Tech Challenge, no qual foi desenvolvido um aplicativo .NET para cadastro de contatos regionais com práticas de CI e monitoramento, esta fase irá introduzir conceitos avançados de arquitetura de microsserviços e mensageria.

## Objetivos
Arquitetura de Microsserviços: refatorar o aplicativo existe em um conjunto de microsserviços em que tenhamos as funcionalidades separadas por contexto. Ex.: o cadastro ser um microsserviço que envia dados para um outro microsserviço persistir os dados.

Comunicação Assíncrona com Mensageria: implementar comunicação assíncrona usando RabbitMQ para eventos entre os microsserviços. Ex.: adicionar o RabbitMQ para comunicação entre os microsserviços.

## Fluxo
Ter um microsserviço que recebe os dados e envie para uma fila.

A fila receber os dados.

Ter um microsserviço consumer que receba os dados e persista eles.