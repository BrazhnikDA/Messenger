#pragma comment(lib, "Ws2_32.lib")
#include <WinSock2.h>
#include <iostream>
#include <WS2tcpip.h>

SOCKET Connect;
SOCKET* Connections;
SOCKET Listen;

int ClientCount = 0;

void SendMessageClient(int id)
{
	char* buffer = new char[1024];
	for (;; Sleep(50))
	{
		memset(buffer, 0, sizeof(buffer));
		if (recv(Connections[id], buffer, 1024, NULL))
		{
			std::cout << buffer << std::endl;
			for (int i = 0; i <= ClientCount; i++)
			{
				send(Connections[i], buffer, strlen(buffer), NULL);
			}
		}
	}
	delete[] buffer;
}

int main()
{
	setlocale(LC_ALL, "Rus");
	WSADATA wsaData;
	WORD DLLVersion = MAKEWORD(2, 2);
	if (WSAStartup(DLLVersion, &wsaData) != 0)  // Проверка на удачную загрузку библиотеки
	{
		std::cout << "#1 Eror include library\n";
		exit(1);
	}

	struct addrinfo hints;
	struct addrinfo* result;

	Connections = (SOCKET*)calloc(64, sizeof(SOCKET));
	//Connections = new SOCKET(64);

	ZeroMemory(&hints, sizeof(hints));

	hints.ai_family = AF_INET;
	hints.ai_flags = AI_PASSIVE;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	getaddrinfo(NULL, "7770", &hints, &result);

	Listen = socket(result->ai_family, result->ai_socktype,result->ai_protocol);
	bind(Listen, result->ai_addr, result->ai_addrlen);
	listen(Listen, SOMAXCONN);

	freeaddrinfo(result);

	std::cout << "Start server!\n";
	char m_connect[] = "Connect...;;;5";
	for (;;Sleep(50)) {
		

		if (Connect = accept(Listen, NULL, NULL))
		{
			std::cout << "Client " << ClientCount << " connected!" << std::endl;

			Connections[ClientCount] = Connect;
			send(Connections[ClientCount], m_connect, strlen(m_connect), NULL);
			CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)SendMessageClient, (LPVOID)(ClientCount), NULL, NULL);
			ClientCount++;
		}
		else
		{
			std::cout << "#2 Eror connection\n";
			system("pause");
			return -1;
		}
	}

	system("pause");
	return 0;

}