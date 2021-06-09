set -e

# cd /root/docker/jenkins/workspace/filezip/svn/Publish/DevOps/filezip; bash 20.docker-build.sh


#(x.1)当前路径 
curWorkDir=$PWD
curPath=$(dirname $0)

cd $curPath/../../..
codePath=$PWD
# codePath=/root/docker/jenkins/workspace/filezip/svn


 
# export DOCKER_USERNAME=serset
# export DOCKER_PASSWORD=xxx
export name=filezip
export projectPath='FileZip'




echo "(x.2)get version"
#version=1.1.0.53
cd $codePath
version=`grep '<Version>' FileZip/FileZip.csproj | grep -o '[0-9\.]\+'`

# echo $version





echo "(x.3)发布dotnet项目"
tag=$version
echo 发布项目 $name:$tag

docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "
cd '/root/code/$projectPath'; 
dotnet build --configuration Release; 
dotnet publish --configuration Release --output '/root/code/Publish/06.Docker/制作镜像/$name/app' " 





#---------------------------------------------------------------------
#(x.4.1)初始化构建器

#启用 buildx 插件
export DOCKER_CLI_EXPERIMENTAL=enabled

#验证是否开启
docker buildx version

#启用 binfmt_misc
docker run --rm --privileged docker/binfmt:66f9012c56a8316f9244ffd7622d7c21c1f6f28d

#验证是 binfmt_misc 否开启
ls -al /proc/sys/fs/binfmt_misc/


#创建一个新的构建器
docker buildx create --use --name mybuilder

#启动构建器
docker buildx inspect mybuilder --bootstrap

#查看当前使用的构建器及构建器支持的 CPU 架构，可以看到支持很多 CPU 架构：
docker buildx ls



#---------------------------------------------------------------------
#(x.4.2)构建多架构镜像（ arm、arm64 和 amd64 ）并推送到 Docker Hub

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD

cd $codePath/Publish/06.Docker/制作镜像/$name
docker buildx build . -t $DOCKER_USERNAME/$name:$tag -t $DOCKER_USERNAME/$name --platform=linux/amd64,linux/arm64,linux/arm/v7 --push
 




 


#(x.5)
cd $curWorkDir

 
