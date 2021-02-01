# FileZip
> FileZip为net core2.1开发的文件加解压工具，包括 文件合并、加压（zip） 和 解压（unzip、un7z、unrar）等功能  
> 运行环境 dotnet core 2.1

## 1. demo
``` bash
# 查看帮助
dotnet FileZip.dll

# 合并文件
dotnet FileZip.dll marge -i "E:\t 2" -o "E:\t 2.7z"

# 解压7z文件到文件夹"E:\t\t2"
dotnet FileZip.dll un7z -i "E:\t\m.7z" -o "E:\t\t2"
```

## 2. docker运行
``` bash
# 查看帮助
docker run --rm -it serset/filezip


# 解压7z文件m.7zd到文件夹 /root/docker/file/a
cd /root/docker/file
docker run --rm -it \
-v $PWD:/root/app/file  \
serset/filezip  \
dotnet FileZip.dll un7z \
"-i" "/root/app/file/m.7z" \
"-o" "/root/app/file/a"

```

## 3. 命令说明：
---------------
help  
帮助文档：  
-c[--command] 要查询的命令。若不指定则返回所有命令的文档。如 help  
示例： help -c help  
---------------
zip  
压缩为gzip文件。参数说明：  
-i[--input] 待压缩目录 例如 "/data/a"  
-o[--output] 压缩后文件名(若不指定,加压到压缩文件夹所在目录),例如 "/data/a.zip"  
示例： zip -i "/data/a" -o "/data/a.zip"  
---------------
marge  
合并文件。参数说明：  
-i[--input] 待合并的文件夹 或 文件查询字符串。 如 /data/a/1.7z.\*    
-o[--output] 合并后文件  
示例： marge -i "/data/a" -o /data/a.7z  
---------------
un7z  
解压7z文件。参数说明：  
-i[--input] 待解压文件 例如 "/data/a.7z.001"  
-o[--output] 输出目录，(若不指定,则解压到压缩文件所在目录)  
-m[--mute] 若指定，则不输出解压的文件（夹）信息  
-mf[--mutefile] 若指定，则不输出解压的文件信息  
示例： un7z -i "/data/a.7z" -o "/data/out" --mute  
---------------
unrar  
解压rar文件。参数说明：  
-i[--input] 待解压文件 例如 "/data/a.rar.001"  
-o[--output] 输出目录，(若不指定,则解压到压缩文件所在目录)  
-m[--mute] 若指定，则不输出解压的文件（夹）信息  
-mf[--mutefile] 若指定，则不输出解压的文件信息  
示例： unrar -i "/data/a.rar" -o "/data/out" --mute  
---------------
unzip  
解压zip文件。参数说明：  
-i[--input] 待解压文件 例如 "/data/a.zip.001"  
-o[--output] 输出目录，(若不指定,则解压到压缩文件所在目录)  
-m[--mute] 若指定，则不输出解压的文件（夹）信息  
-mf[--mutefile] 若指定，则不输出解压的文件信息  
示例： unzip -i "/data/a.zip" -o "/data/out" --mute  
---------------