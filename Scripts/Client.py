"""
    Entry Points for Packages:

    If you develop a reusable package with multiple modules, you can define functions or scripts as entry points.
    These entry points are typically configured in setup.py using the entry_points argument.
    Users can then install your package and execute those functions directly from the command line using specific names.
    For example, you could define an entry point to run a data analysis script from your package:

    [entry_points]
    console_scripts = [
        my_package_analysis = "my_package.analysis_script:main"
    ]

"""
import socket
import sys

def send_data(sock, message):
    print(f"Sending: {message}")
    sock.sendall(message.encode())

def receive_data(sock):
    sock.settimeout(2.0)
    try:
        data = sock.recv(1024)
        print(f"Received: {data.decode()}")
    except socket.timeout:
        print("No response received within timeout period.")
    finally:
        sock.settimeout(None)

def create_tcp_client(server_ip, server_port):
    while True:
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        try:
            sock.connect((server_ip, server_port))
            print(f"Connected to {server_ip} on port {server_port}")
            while True:
                message = input("Enter message to send (or type 'exit' to quit): ")
                if message.lower() == 'exit':
                    break
                send_data(sock, message)
                receive_data(sock)
            break  # Exit the outer loop if 'exit' command is given
        except Exception as e:
            print(f"An error occurred: {e}")
        finally:
            print("Closing socket.")
            sock.close()
        # If needed, implement a reconnect strategy or a break condition here

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python client.py <server_ip> <server_port>")
    else:
        server_ip = sys.argv[1]
        server_port = int(sys.argv[2])
        create_tcp_client(server_ip, server_port)
