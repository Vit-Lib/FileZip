set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn


export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`



# "

 

#----------------------------------------------
echo "(x.2)dotnet-publish"
echo "dotnet version: ${netVersion}"

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
set -e

basePath=/root/code
publishPath=\$basePath/Publish/release/release/publish

#(x.3)查找所有需要发布的项目并发布
cd \$basePath
for file in \$(grep -a '<publish>' . -rl --include *.csproj)
do
	cd \$basePath
	
	#get publishName
	publishName=\`grep '<publish>' \$file -r | grep -oP '>(.*)<' | tr -d '<>'\`

	echo publish \$publishName

	#publish
	cd \$(dirname \"\$file\")
	dotnet build --configuration Release
	dotnet publish --configuration Release --output \$publishPath/\$publishName
done


#(x.4)copy bat
\\cp -rf /root/code/Publish/ReleaseFile/publish/. \$publishPath


"




echo 'publish succeed！'






