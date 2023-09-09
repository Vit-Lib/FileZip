FROM serset/dotnet:runtime-6.0


#(x.1)install zip unzip
RUN apt-get update && apt-get -y install zip unzip

#(x.2)alias filezip
RUN echo '#!/bin/bash\ndotnet /root/filezip/FileZip.dll $*' > /usr/bin/filezip && chmod +x /usr/bin/filezip

#(x.3)app
COPY app /root/filezip
WORKDIR /root/filezip
CMD dotnet /root/filezip/FileZip.dll help