# 服务代理
## 项目说明
|项目|名称|地址|说明|
|-|-|-|-|
|Client1|客户端|自动|产生请求|
|Proxy1|代理服务|127.0.0.1:8001|读取客户端数据，转发给目标服务|
|Target1|目标服务|127.0.0.1:8002|读取数据，并响应|
## 示意图
**请求流程**：Client1 --> Proxy1 --> Target1
**响应流程**：Target1 --> Proxy1 --> Client1
## 客户端请求标头
```
POST http://127.0.0.1:8002/ HTTP/1.1
Host: 127.0.0.1:8002
Content-Type: application/json
Content-Length: 24

来自客户端的消息
```
## 代理服务实现逻辑
客户端请求标头中包含目标服务的信息：`Host: 127.0.0.1:8002`。
代理服务读取到这数据，就能向目标服务进行网络请求。
## 有哪些代理服务
### 正向代理
- Squid
- Microsoft ISA Server
- Blue Coat ProxySG
- F5 BIG-IP
### 反向代理
- **Nginx**
- **Apache**
- HAProxy
- Traefik