set -e


#---------------------------------------------------------------------
#(x.1)参数
args_="

export codePath=/root/temp/svn


# "

 


#---------------------------------------------------------------------
echo "(x.2)docker-image-create"


publishPath=$codePath/Publish/release/release/publish
dockerPath=$codePath/Publish/release/release/docker-image



echo "copy dir"
mkdir -p $dockerPath
\cp -rf $codePath/Publish/ReleaseFile/docker-image/. $dockerPath


#查找所有需要发布的项目并发布
cd $codePath
for file in $(grep -a '<docker>' . -rl --include *.csproj)
do
	cd $codePath
	
	#get publishName
	publishName=`grep '<publish>' $file -r | grep -oP '>(.*)<' | tr -d '<>'`
	
	#get dockerName
	dockerName=`grep '<docker>' $file -r | grep -oP '>(.*)<' | tr -d '<>'`

	echo create $dockerName

	#copy file
	\cp -rf "$publishPath/$publishName" "$dockerPath/$dockerName/app"
done


