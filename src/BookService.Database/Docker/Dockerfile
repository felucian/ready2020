FROM microsoft/mssql-server-linux:2017-CU12
RUN apt-get update && apt-get -q install -y curl
RUN apt-get -q install -y net-tools
RUN apt-get -q install -y dnsutils
RUN apt-get -q install -y iputils-ping
RUN apt-get -q install -y inetutils-traceroute
ENV SA_PASSWORD=Passw0rd
ENV ACCEPT_EULA=Y
COPY DbInit.sql DbInit.sql
COPY entrypoint.sh entrypoint.sh
COPY sqlcmdStartup.sh sqlcmdStartup.sh
RUN chmod +x sqlcmdStartup.sh
CMD /bin/bash ./entrypoint.sh