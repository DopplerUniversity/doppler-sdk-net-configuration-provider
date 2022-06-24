SHELL:=/usr/bin/env bash

doppler-client-dev-file:
	@echo "{ \"DopplerToken\": \""$$(doppler configs tokens create dev --plain)"\" }" > dopplerClientConfig.Development.json

dotnet-run:
	DopplerToken="$$(doppler configs tokens create  dev --max-age 1m --plain)" dotnet run