# Alert: UWP behavior: UWP may block loopback packets
##
### UWP Server instances may not responding loopback connect requests, additional steps is needed for connecting to localhost. 
## Reference: 
### <https://learn.microsoft.com/en-us/windows/security/operating-system-security/network-security/windows-firewall/troubleshooting-uwp-firewall>
## Resolution:
### Prompt command below (Powershell, Admin needed) before starting the emulator
```CheckNetIsolation.exe LoopbackExempt -is -n=85b7304a-465a-4d02-b27f-bf4f122704ec_agrbw38z0xjy8```