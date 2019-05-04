#!/bin/bash

gnome-terminal --tab --title="Catalog" -e "bash --rcfile rcfiles/catalog" \
               --tab --title="Basket" -e "bash --rcfile rcfiles/basket" \
               --tab --title="Ordering" -e "bash --rcfile rcfiles/ordering" \
               --tab --title="Payment" -e "bash --rcfile rcfiles/payment" \
               --tab --title="Identity" -e "bash --rcfile rcfiles/identity" \
               --tab --title="API GW" -e "bash --rcfile rcfiles/apigw" \
               --tab --title="Agg" -e "bash --rcfile rcfiles/agg" \
               --tab --title="SignalR" -e "bash --rcfile rcfiles/signalr"

exit 0