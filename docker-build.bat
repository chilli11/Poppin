
cd emberjs/poppin-ui
CALL ember build --environment=production

cd ../../
CALL rmdir /q /s dockerpub
CALL dotnet publish -c Release -o dockerpub
CALL docker build -t poppin-app .
CALL aws ecr get-login-password --region us-west-2 | docker login --username AWS --password-stdin 750171440750.dkr.ecr.us-west-2.amazonaws.com
CALL docker tag poppin-app:latest 750171440750.dkr.ecr.us-west-2.amazonaws.com/poppin-app:latest
CALL docker push 750171440750.dkr.ecr.us-west-2.amazonaws.com/poppin-app:latest

PAUSE