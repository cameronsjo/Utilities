#!/bin/sh

sudo apt-get update --allow-releaseinfo-change
sudo apt-get update && sudo apt-get upgrade
sudo hb-service update-node
