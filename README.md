# FileZip
> FileZip为net core2.1开发的文件加解压工具，包括 文件合并、加压（zip） 和 解压（unzip）文件
> 运行环境 dotnet core 2.1

## 1. demo
``` bash
# 查看帮助
dotnet FileZip.dll

# 合并文件
dotnet FileZip.dll marge -i "E:\t 2" -o "E:\t 2.7z"

# 解压7z文件到文件夹"E:\t\t2"
dotnet FileZip.dll unzip -i "E:\t\m.7z" -o "E:\t\t2"
```

## 2. docker运行
``` bash
# 查看帮助
docker run --rm -it serset/filezip


# 解压文件m.7z到文件夹 /root/docker/file/m
docker run --rm -it -v /root/docker/file:/root/file serset/filezip  dotnet FileZip.dll unzip -i /root/file/m.7z -o /root/file/m

```

## 3. 命令说明：
``` txt
---------------
help
帮助文档：
-c[--command] 要查询的命令。若不指定则返回所有命令的文档。如 help
示例： help -c help
---------------
marge
合并文件。参数说明：
-i[--input] 待合并的文件夹 或 文件查询字符串。 如 /data/a/1.7z.*
-o[--output] 合并后文件
示例： marge -i "/data/a" -o /data/a.7z
---------------
unzip
解压zip文件。若为分卷（后缀为数字 如.001）则先合并文件再解压
支持格式: zip,gz,bzip2,tar,rar,lzip,xz,7zip,7z
参数说明：
-i[--input] 待解压文件 例如 "/data/a.zip.001"
-o[--output] 输出目录，(若不指定,则解压到压缩文件所在目录)
-p[--progress] 进度信息间隔。如0.1代表进度每增加10%，提示一次。默认0.1
-f[--file] 若指定，则输出每一个文件信息
-d[--dir] 若指定，则输出每一个文件夹信息
示例： unzip -i "/data/a.zip" -o "/data/out" -p 0.1 -f -d
---------------
zip
压缩文件(夹)
支持格式: .zip, .gz, .tar
参数说明：
-i[--input] 待压缩文件(夹),例如 "/data/a.txt" "/data/files"
-o[--output] 压缩输出文件m，使用后缀指定格式压缩，(若不指定,输出到待压缩文件所在目录)
-p[--progress] 进度信息间隔。如0.1代表进度每增加10%，提示一次。默认0.1
-f[--file] 若指定，则输出每一个文件信息
示例： zip -i "/data/files" -o "/data/files.zip" -p 0.1 -f
---------------
``` 