all: Demo

DEFINES += -DMEMTRACE_USE_TEMP_FILE

.cpp.o:
	clang++ -c $(DEFINES) -std=c++11 -Wall -o $@ $<

MemTrace.o: MemTrace.cpp MemTrace.h MemTraceSys.h MemTraceInit.h

MemTraceSys.o: MemTraceSys.cpp MemTrace.h MemTraceSys.h

DemoMain.o: DemoMain.cpp MemTrace.h MemTraceInit.h

Demo: DemoMain.o MemTrace.o MemTraceSys.o
	clang++ -std=c++11 -Wall -o $@ $^

clean:
	rm -f Demo *.o
