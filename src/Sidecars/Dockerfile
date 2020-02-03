FROM envoyproxy/envoy:v1.12.2
RUN apt-get update && apt-get -q install -y curl
RUN apt-get -q install -y net-tools
RUN apt-get -q install -y dnsutils
RUN apt-get -q install -y iputils-ping
RUN apt-get -q install -y inetutils-traceroute
RUN apt-get -q install -y tcpdump
RUN apt-get -q install -y netcat
RUN apt-get -q install -y nano
#CMD /usr/local/bin/envoy -c /etc/envoy/envoy.yaml  --service-cluster test --service-node test