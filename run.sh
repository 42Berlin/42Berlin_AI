#!/usr/bin/bash

DEFAULT_PATH="/mnt/c/Users/42Bear/Documents/42BearPlayground/42Berlin_AI/Builds/42AI/42AI.exe";
DEFAULT_PATH_DEV="/mnt/c/Users/42Bear/Documents/42BearPlayground/42Berlin_AI/Builds/42AI_dev/42AI.exe";

FINAL_PATH="";

RED='\033[0;31;01m'
GREY='\033[0;30;01m'
WHITE='\033[0;37;01m'
GREEN='\033[0;32;01m'
YELLOW='\033[0;33;01m'
BLUE='\033[0;34;01m'
C_END='\033[0m'

VERBOSE=0

# Run the application

for arg in "$@"
do
	if [[ $arg == "dev" ]] ; then
		FINAL_PATH=$DEFAULT_PATH_DEV
	elif [[ $arg == "-v" ]] || [[ $arg == "--verbose" ]] ; then
		VERBOSE=1
	elif [[ $arg == "-h" ]] || [[ $arg == "--help" ]] ; then
		echo "Usage: sh run_test.sh [EXEC_PATH] [dev]"
		#echo "${RED}Usage: ${WHITE}sh run_test.sh [EXEC_PATH] [dev] ${C_END}"
	elif [[ -n $arg ]] ; then
		FINAL_PATH=$arg
	fi
done

if [[ -z $FINAL_PATH ]] ; then
	FINAL_PATH=$DEFAULT_PATH
fi


if [[ $VERBOSE == 1 ]] ; then
	echo "Running: $FINAL_PATH"
	# echo "${GREEN}Running: ${WHITE}$FINAL_PATH${C_END}"
fi


. ./42Berlin_AI_server/venv/bin/activate && cd 42Berlin_AI_server ; $FINAL_PATH -profiler-maxusedmemory 512000000 & sh launch.sh ; fg

