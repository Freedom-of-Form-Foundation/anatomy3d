# Find all sources in subdirectories:
SOURCES := $(wildcard *.cs) $(wildcard */*.cs) $(wildcard */*/*.cs) $(wildcard */*/*/*.cs)

REFERENCES := System.Numerics

space :=
space +=

test.exe: ; @mcs $(addsuffix .dll,$(addprefix /reference:,$(REFERENCES))$)$ $(space) $(SOURCES)
