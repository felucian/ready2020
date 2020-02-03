k delete cm bookinfospasidecar  bookinfospanoressidecar  bookservicesidecar bookservicenoressidecar

Set-Location K8sConfigurations\configmaps
### GRAFANA
kubectl create configmap grafanadatasource --from-file=Grafana/datasources.yaml
kubectl create configmap grafanaconfig --from-file=Grafana/dashboards.yaml
kubectl create configmap grafanadashboards --from-file=Grafana/successrateDashboard.json --from-file=Grafana/bookserviceDashboard.json
kubectl create configmap grafanaini --from-file=Grafana/grafana.ini
### SIDECARS
kubectl create configmap bookinfospasidecar --from-file=Sidecars/bookinfospa-sidecar/envoy.yaml
kubectl create configmap bookinfospanoressidecar --from-file=Sidecars/bookinfospa-noresiliencysidecar/envoy.yaml
kubectl create configmap bookservicesidecar --from-file=Sidecars/bookservice-sidecar/envoy.yaml
kubectl create configmap bookservicenoressidecar --from-file=Sidecars/bookservice-noresiliencysidecar/envoy.yaml
Set-Location ..\..


##REPLACE CONFIGMAPS
#kubectl create configmap grafanadashboards --from-file=Grafana/successrateDashboard.json -o yaml --dry-run | kubectl replace -f -

#kubectl create configmap bookservicesidecar --from-file=Sidecars/bookservice-sidecar/envoy.yaml -o yaml --dry-run | kubectl replace -f -