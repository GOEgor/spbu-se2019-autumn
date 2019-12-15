#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include "bitonic_sorts.cuh"

#define TIME_TESTS 5
#define MAX_RAND 10000
#define MAX_EXP 25

void generate_arr(int *arr, int n)
{
	srand(time(NULL));
	for (int i = 0; i < n; i++)
		arr[i] = (rand() % MAX_RAND);
}

void verify(int *arr, int n)
{
	for (int i = 0; i < n - 1; i++)
		if (arr[i] > arr[i + 1])
		{
			printf("ERROR IN SORT!!!1\n");
			return;
		}

	printf("Sort is correct.\n");
}

int main()
{
	cudaFree(0);

	for (unsigned int exp = 1; exp <= MAX_EXP; exp++)
	{
		int len = 1 << exp;
		int* test_arr = (int *) malloc(sizeof(int) * len);	

		if (test_arr == NULL)
		{
			printf("Cannot allocate memory for test array.\n");
			exit(1);
		}
		
		generate_arr(test_arr, len);

		clock_t start, end;
		double cpu_time_used = 0;
		double gpu_time_used = 0;

		for (int i = 0; i < TIME_TESTS; i++)
		{
			int* arr_cpu_copy = (int *) malloc(sizeof(int) * len);
			int* arr_gpu_copy = (int *) malloc(sizeof(int) * len);

			memcpy(arr_cpu_copy, test_arr, len);
			memcpy(arr_gpu_copy, test_arr, len);

			start = clock();
			bitonic_sort_default(arr_cpu_copy, exp);
			end = clock();

			cpu_time_used += ((double)(end - start)) / CLOCKS_PER_SEC;

			start = clock();
			bitonic_sort_gpu(arr_gpu_copy, exp);
			end = clock();

			gpu_time_used += ((double)(end - start)) / CLOCKS_PER_SEC;

			free(arr_cpu_copy);
			free(arr_gpu_copy);
		}

		double cpu_time_avg = cpu_time_used / TIME_TESTS;
		double gpu_time_avg = gpu_time_used / TIME_TESTS;

		printf("%d %f %f\n", len, cpu_time_avg, gpu_time_avg);		

		bitonic_sort_gpu(test_arr, exp);
		verify(test_arr, len);

		free(test_arr);
	}

	if (cudaDeviceReset() != cudaSuccess) {
		printf("cudaDeviceReset failed!");
		exit(1);
	}

	return 0;
}