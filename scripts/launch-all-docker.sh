#!/bin/bash

gnome-terminal --tab --title="Catalog" -e "bash --rcfile rcfiles-docker/catalog" \
               --tab --title="Basket" -e "bash --rcfile rcfiles-docker/basket" \
               --tab --title="Ordering" -e "bash --rcfile rcfiles-docker/ordering" \
               --tab --title="Payment" -e "bash --rcfile rcfiles-docker/payment" \
               --tab --title="Identity" -e "bash --rcfile rcfiles-docker/identity" \
               --tab --title="API GW" -e "bash --rcfile rcfiles-docker/apigw" \
               --tab --title="Agg" -e "bash --rcfile rcfiles-docker/agg" \
               --tab --title="SignalR" -e "bash --rcfile rcfiles-docker/signalr"

exit 0