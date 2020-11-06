
cd emberjs/poppin-ui
CALL ember build --environment=production

cd ../../
CALL dotnet publish -c Release -o dockerpub
CALL docker build -t poppin-app .
CALL docker tag poppin-app:latest 750171440750.dkr.ecr.us-west-2.amazonaws.com/poppin-app:latest
CALL docker push 750171440750.dkr.ecr.us-west-2.amazonaws.com/poppin-app:latest

PAUSE