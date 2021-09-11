#!/bin/sh

 # apt-get update --allow-releaseinfo-change
 sudo apt-get update && sudo apt-get upgrade
 sudo pihole -up
