#!/bin/sh

# Creates Ubuntu webserver to run x.org 
# test site is: https://hmsoftware.org/

# deal with files just copied by scp from infra.azcli

sudo mkdir /certs
sudo chown -R dave:dave /certs
sudo chmod +rw /certs

cd /home/dave
sudo mv *.key /certs
sudo mv *.pem /certs


# disable auto upgrades by apt 
# may be okay with this pure webserver

# cat <<EOT >> 20auto-upgrades
# APT::Periodic::Update-Package-Lists "0";
# APT::Periodic::Download-Upgradeable-Packages "0";
# APT::Periodic::AutocleanInterval "0";
# APT::Periodic::Unattended-Upgrade "1";
# EOT

# sudo mv /home/dave/20auto-upgrades /etc/apt/apt.conf.d/20auto-upgrades

# go with newer apt which gets dependency updates too (like linux-azure)
sudo apt update -y
sudo apt upgrade -y

  
# Install packages for .NET for Ubutu 20.04 LTS
# wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
# sudo dpkg -i packages-microsoft-prod.deb
# rm packages-microsoft-prod.deb

# nginx
# we're using Kestrel as the webserver to service .NET
# nginx as a reverse proxy for the following reasons 
# mainly it is easily to get certs installed, and option to have multiple sites on this ip or subdomains
# eg potentially a wordpress site for future?
# https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/when-to-use-a-reverse-proxy?view=aspnetcore-7.0
sudo apt-get install nginx -y

# .NET 6 SDK
# sudo apt-get update; \
#   sudo apt-get install -y apt-transport-https && \
#   sudo apt-get update && \
#   sudo apt-get install -y dotnet-sdk-6.0

# .NET 7.0.105 SDK
# includes 7.0.5 runtime
# https://learn.microsoft.com/en-gb/dotnet/core/install/linux-ubuntu-2204
sudo apt-get install -y dotnet-sdk-7.0


# create document root for published files 
# it is there already with an html subdirectory
# sudo mkdir /var/www

# create gitsource folder and clone
sudo mkdir /gitsource
cd /gitsource
sudo git clone https://github.com/djhmateer/vll .


# nginx config
# ssl certs will already be in /certs
# copied in with create-kestrel-osr-with-secrets.sh file
sudo cp /gitsource/infra/nginx.conf /etc/nginx/sites-available/default
sudo nginx -s reload

# compile and publish the web app
sudo dotnet publish /gitsource/src/VLL.Web --configuration Release --output /var/www

# change ownership of the published files to what it will run under
sudo chown -R www-data:www-data /var/www
# allow exective permissions
sudo chmod +x /var/www

# cookie keys to allow machine to restart and for it to 'remember' cookies
# DM ****HERE****
# sudo mkdir /var/cookie-keys
# sudo chown -R www-data:www-data /var/cookie-keys
# allow read and write
# sudo chmod +rw /var/cookie-keys

# fileshare for cookies

# sudo mkdir /osrFileStore
# sudo chown -R www-data:www-data /osrFileStore

sudo mv /home/dave/vllshare.cred /var/

sudo mkdir /mnt/vllshare

# how to get the mount to survive a reboot
# https://learn.microsoft.com/en-us/azure/storage/files/storage-how-to-use-files-linux?tabs=Ubuntu%2Csmb311#automatically-mount-file-shares
# allow all local users access to the share
# https://unix.stackexchange.com/a/375523/278547
echo "//vllstorageaccount.file.core.windows.net/vllshare /mnt/vllshare cifs nofail,noperm,credentials=/var/vllshare.cred,serverino,nosharesock,actimeo=30" | sudo tee -a /etc/fstab > /dev/null

# reload fstab
sudo mount -a


# https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-5.0#configure-the-firewall
sudo apt-get install ufw

sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

sudo ufw enable

  # a nice shortcut sym link
# sudo ln -s /usr/local/openresty/nginx/ /home/dave/nginx
sudo ln -s /var/www/logs/ /home/dave/logs

# sudo snap install bpytop

# auto start the service
sudo mv /home/dave/kestrel.service /etc/systemd/system/kestrel.service
sudo chmod 644 /etc/systemd/system/kestrel.service

# auto start on machine reboot
sudo systemctl enable kestrel.service
sudo systemctl restart kestrel.service


# boot????

