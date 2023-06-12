# The SneezeBoardServer.exe is currently running at Google Compute Engine
# in the account christensen.darren@gmail.com
#

# If the SneezeBoardServer.exe is down it usually will display an error in the clien
# "Could not establish at connection at the specified Ip Address"
#
# To confirm SSH into the server and run the command:
ps Sneeze* 

# If it is not running restart it at the root directory with the command
sudo nohup mono SneezeBoardServer.exe

# sudo gives the command elevated privelages
# nohup will keep the executable running and not listen for the key event to shutdown 
# mono specifies to run this windows app under mono


# If you need to get a backup of the data, connect to the server via SSH
cd ..
cd ..
cd usr/share/SneezeBoard

# This directory holds the database.xml file and backups.  If you are using the SSH-in_browser 
# from Google, you can download each file with the download file button on the top navigation

# If you need to kill the SneezeBoardServer you can first determine the pid for the running application
ps ax
# Find the process "sudo nohup mono SneezeBoardServer.exe" and run the following command by replacing 
# PID with the actual PID number
sudo ntxkillx PID
