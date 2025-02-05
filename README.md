**Desenho da Solução MVP**

## **1. Introdução**
Este documento apresenta o desenho da solução MVP para o sistema de Telemedicina da Health&Med. O objetivo é garantir um sistema robusto, escalável e seguro, atendendo aos requisitos funcionais especificados.

## **2. Arquitetura da Solução**
![image](https://github.com/user-attachments/assets/28fba7a8-3ec1-493b-9468-e563e1007f2e)

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
- **Sql Server**: Banco relacional.

### **3.4 Autenticação**
- **Keycloak/Auth0**: Soluções seguras e prontas para uso, seguindo padrões como OAuth2 e OpenID Connect.

### **3.5 Infraestrutura e Orquestração**
- **AWS/Azure**:
- **Docker + Kubernetes**:


