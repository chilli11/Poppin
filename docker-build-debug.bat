
cd emberjs/poppin-ui
CALL ember build --environment=development

cd ../../
CALL rmdir /q /s dockerpub
CALL dotnet publish -c Debug -o ./dockerpub /p:EnvironmentName=Development
CALL docker build -t poppin-app-debug .
CALL aws ecr get-login-password --region us-west-2 | docker login --username AWS --password-stdin 750171440750.dkr.ecr.us-west-2.amazonaws.com
CALL docker tag poppin-app-debug:latest 750171440750.dkr.ecr.us-west-2.amazonaws.com/poppin-app-debug:latest
CALL docker push 750171440750.dkr.ecr.us-west-2.amazonaws.com/poppin-app-debug:latest

PAUSE