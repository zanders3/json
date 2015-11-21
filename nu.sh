#!/bin/bash

set -x

rm -rf packages/

nuget install -o packages/
