type NUL > testresults.txt

gcc gauss_solutions.c -o sols -lgsl -lgslcblas -fopenmp -O0

set sizes=50,100,500,1000,2000,3000

for %%i in (%sizes%) do (
    sols.exe %%i >> testresults.txt
)