# ProxyChecker
A high-performance proxy checker built in C#. This proxy supports HTTP/s and SOCKS protocols. 

## Usage
```
Proxychecker [OPTIONS] value
Options:
--read | -r         : Species file source or url
--write | -w        : Species output file destination (default file would be {unix}.txt)
--threads | -t      : Number of threads (5 by default)
--proxy-type | -p   : socks4, socks5, http/s (http by default)
--print             : Prints a file content
--help | -h         : This output
```
