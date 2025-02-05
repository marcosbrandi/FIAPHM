**Desenho da Solução MVP**

## **1. Introdução**
Este documento apresenta o desenho da solução MVP para o sistema de Telemedicina da Health&Med.

A aplicação tem como principal objetivo possibilitar o agendamento de consultas médicas, com as seguintes funcionalidades:

Permitir o cadastro de novos médicos e pacientes.
O paciente terá acesso a agenda de médicos disponíveis, e às suas respectivas agendas. Podendo agendar uma consulta a qualquer momento.

## **2. Arquitetura da Solução**
![image](https://github.com/user-attachments/assets/59772eb7-f631-4b7e-a6cc-034716ab15af)

### **2.1 Componentes Principais**
- **Backend**: API REST ASP.NET Core.
- **Banco de Dados**: SQL Server.
- **Autenticação**: JWT Bearer Token.
- **Infraestrutura**: ?????.

### **2.2 Fluxo de Dados**
1. Paciente/Médico solicita autenticação.
2. Quando médico, cadastra agenda, quando Paciente, agenda consulta.
3. Backend interage com o banco de dados Sql Server.

### **3 Backend**
- **.NET Core**

### **3.1 Banco de Dados**
- **Sql Server**

### **3.2 Autenticação**
- **Keycloak/Auth0**: Soluções seguras e prontas para uso, seguindo padrões como OAuth2 e OpenID Connect.

### **3.4 Infraestrutura e Orquestração**
- **AWS/Azure**:
- **Docker + Kubernetes**:


