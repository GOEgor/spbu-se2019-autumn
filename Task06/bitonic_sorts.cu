#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#define NUM_THREADS 512
//https://stackoverflow.com/questions/14038589/what-is-the-canonical-way-to-check-for-errors-using-the-cuda-runtime-api
#define gpuErrCheck(ans) { gpuAssert((ans), __FILE__, __LINE__); }
inline void gpuAssert(cudaError_t code, const char *file, int line, bool abort = true)
{
	if (code != cudaSuccess)
	{
		fprintf(stderr, "GPUassert: %s %s %d\n", cudaGetErrorString(code), file, line);
		if (abort) exit(code);
	}
}

void bitonic_sort_default(int *arr, unsigned int log_len) 
{
	int size = 1 << log_len;

	for (int seq = 2; seq <= size; seq <<= 2) 
	{
		for (int dist = seq >> 2; dist > 0; dist >>= 2) 
		{
			for (int item = 0; item < size; item++) 
			{
				int pair_item = item | dist;

				if (((item & seq) == 0) && (arr[item] > arr[pair_item])
					|| ((item & seq) != 0) && (arr[item] < arr[pair_item]))
				{
					int temp = arr[item];
					arr[item] = arr[pair_item];
					arr[pair_item] = temp;
				}
			}
		}
	}
}

__global__ void bitonic_sort_step(int *arr, int dist, int seq) 
{
	int item = threadIdx.x + blockIdx.x * blockDim.x;
	int pair_item = item | dist;

	if (((item & seq) == 0) && (arr[item] > arr[pair_item])
		|| ((item & seq) != 0) && (arr[item] < arr[pair_item]))
	{
		int temp = arr[item];
		arr[item] = arr[pair_item];
		arr[pair_item] = temp;
	}
}

void bitonic_sort_gpu(int *arr, unsigned int exp) 
{
	int arr_len = 1 << exp;
	size_t arr_size = sizeof(int) * arr_len;
	int* d_arr;

	gpuErrCheck(cudaMalloc(&d_arr, arr_size));
	gpuErrCheck(cudaMemcpy(d_arr, arr, arr_size, cudaMemcpyHostToDevice));

	int num_blocks = arr_len / NUM_THREADS;
    int num_threads = NUM_THREADS;

	if (arr_len / NUM_THREADS == 0) 
	{
		num_blocks = arr_len;
		num_threads = 1;
	}

	for (int seq = 2; seq <= arr_len; seq <<= 1)
	{
		for (int dist = seq >> 1; dist > 0; dist >>= 1) 
		{
			bitonic_sort_step<<<num_blocks, num_threads>>>(d_arr, dist, seq);
		}
	}

	gpuErrCheck(cudaMemcpy(arr, d_arr, arr_size, cudaMemcpyDeviceToHost));
	cudaFree(d_arr);
}