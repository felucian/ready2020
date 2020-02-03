##delete everything##
kubectl delete deploy bookinfo-spa-resiliency bookservice-resiliency bookinfo-spa-noresiliency bookservice-noresiliency prometheus grafana
kubectl delete svc bookinfo-spa bookservice bookinfo-spa-noresiliency bookservice-noresiliency prometheus grafana
kubectl delete cm bookinfospanoressidecar bookinfospasidecar bookservicenoressidecar bookservicesidecar
kubectl delete cm grafanadatasource grafanaconfig grafanadashboards grafanaini
kubectl delete cm prometheus-rules prometheus-server-conf
kubectl delete secret appinsightskey featureflagkey 
kubectl delete sa monitoring
kubectl delete clusterrolebinding monitoring
#


#
kubectl apply -f K8sConfigurations\secrets\

kubectl create configmap bookinfospasidecar --from-file=K8sConfigurations\configmaps\Sidecars\bookinfospa-retry\envoy.yaml
kubectl create configmap bookinfospanoressidecar --from-file=K8sConfigurations\configmaps\Sidecars\bookinfospa-default\envoy.yaml
kubectl create configmap bookservicesidecar --from-file=K8sConfigurations\configmaps\Sidecars\bookservice-default\envoy.yaml
kubectl create configmap bookservicenoressidecar --from-file=K8sConfigurations\configmaps\Sidecars\bookservice-default\envoy.yaml

kubectl create configmap grafanadatasource --from-file=K8sConfigurations\configmaps\Grafana/datasources.yaml
kubectl create configmap grafanaconfig --from-file=K8sConfigurations\configmaps\Grafana/dashboards.yaml
kubectl create configmap grafanadashboards --from-file=K8sConfigurations\configmaps\Grafana/successrateDashboard.json --from-file=K8sConfigurations\configmaps\Grafana\bookserviceDashboard.json
kubectl create configmap grafanaini --from-file=K8sConfigurations\configmaps\Grafana\grafana.ini

kubectl apply -f K8sConfigurations\default\bookinfo-spa-noresiliency.yaml
kubectl apply -f K8sConfigurations\default\bookinfo-spa.yaml
kubectl apply -f K8sConfigurations\default\bookservice-noresiliency.yaml
kubectl apply -f K8sConfigurations\default\bookservice.yaml
kubectl apply -f K8sConfigurations\default\grafana.yaml

kubectl apply -f K8sConfigurations\prometheus\00-prometheus-rbac.yaml
kubectl apply -f K8sConfigurations\prometheus\01-prometheus-config.yaml
kubectl apply -f K8sConfigurations\prometheus\02-prometheus-rules.yaml
kubectl apply -f K8sConfigurations\prometheus\03-prometheus-deployment.yaml
kubectl apply -f K8sConfigurations\prometheus\04-prometheus-service.yaml
#


##update bookservice with resiliency##
kubectl create configmap bookservicesidecar --from-file=K8sConfigurations\configmaps\Sidecars\bookservice-sidecar-cb\envoy.yaml -o yaml --dry-run | kubectl replace -f -
kubectl delete deploy bookservice-resiliency
kubectl apply -f K8sConfigurations\default\bookservice.yaml

##update bookinfospa with resiliency##
kubectl create configmap bookinfospasidecar --from-file=K8sConfigurations\configmaps\Sidecars\bookinfospa-retry-cb\envoy.yaml -o yaml --dry-run | kubectl replace -f -
kubectl delete deploy bookinfo-spa-resiliency
kubectl apply -f K8sConfigurations\default\bookinfo-spa.yaml
#
