#include <stdio.h>
#include <gsl\gsl_linalg.h>
#include <omp.h>
#include <time.h>
#include <string.h>

#define MAX_RAND 1000000
#define TIME_TESTS 10
#define SOL_PRECISION 1e-9

double *matrix_A;
double *vector_x_seq;
double *vector_x_par;
double *vector_x_gsl;
double *vector_b;

enum solution_type
{
    SEQ, 
    PAR, 
    GSL,
};

void solve_seq(double *matrix_A, double *vector_x, double *vector_b, int size)
{
    double *matrix_A_copy = malloc(sizeof(double) * size * size);
    double *vector_b_copy = malloc(sizeof(double) * size);

    if (matrix_A_copy == NULL || vector_b_copy == NULL)
    {
        printf("Cannot allocate memory for matrix / vector copy.\n");
        exit(1);
    }

    memcpy(matrix_A_copy, matrix_A, size);
    memcpy(vector_b_copy, vector_b, size);

    for (int k = 0; k < size - 1; k++) 
    {
        double pivot = matrix_A_copy[k * size + k];

        for (int i = k + 1; i < size; i++) 
        {
            double multiplier = matrix_A_copy[i * size + k] / pivot;

            for (int j = k; j < size; j++)
                matrix_A_copy[i * size + j] -= multiplier * matrix_A_copy[k * size + j];

            vector_b_copy[i] -= multiplier * vector_b_copy[k];
        }
    }

    for (int k = size - 1; k >= 0; k--)
    {
        vector_x[k] = vector_b_copy[k];
        
        for (int i = k + 1; i < size; i++)
            vector_x[k] -= matrix_A_copy[k * size + i] * vector_x[i];

        vector_x[k] /= matrix_A_copy[k * size + k];
    }


    free(matrix_A_copy);
    free(vector_b_copy);
}

void solve_par(double *matrix_A, double *vector_x, double *vector_b, int size)
{
    double *matrix_A_copy = malloc(sizeof(double) * size * size);
    double *vector_b_copy = malloc(sizeof(double) * size);

    if (matrix_A_copy == NULL || vector_b_copy == NULL)
    {
        printf("Cannot allocate memory for matrix / vector copy.\n");
        exit(1);
    }

    memcpy(matrix_A_copy, matrix_A, size);
    memcpy(vector_b_copy, vector_b, size);

    for (int k = 0; k < size - 1; k++) 
    {
        double pivot = matrix_A_copy[k * size + k];

        for (int i = k + 1; i < size; i++) 
        {
            double multiplier = matrix_A_copy[i * size + k] / pivot;

            #pragma omp simd
            for (int j = k; j < size; j++)
                matrix_A_copy[i * size + j] -= multiplier * matrix_A_copy[k * size + j];

            vector_b_copy[i] -= multiplier * vector_b_copy[k];
        }
    }

    for (int k = size - 1; k >= 0; k--)
    {
        vector_x[k] = vector_b_copy[k];
        
        #pragma omp simd
        for (int i = k + 1; i < size; i++)
            vector_x[k] -= matrix_A_copy[k * size + i] * vector_x[i];

        vector_x[k] /= matrix_A_copy[k * size + k];
    }


    free(matrix_A_copy);
    free(vector_b_copy);
}
void solve_gsl(double* matrix_A, double* vector_x, double* vector_b, int size)
{
    gsl_matrix_view gsl_a = gsl_matrix_view_array(matrix_A, size, size);
    gsl_vector_view gsl_b = gsl_vector_view_array(vector_b, size);
    gsl_vector *gsl_x = gsl_vector_alloc(size);

    int s;

    gsl_permutation *p = gsl_permutation_alloc(size);
    gsl_linalg_LU_decomp(&gsl_a.matrix, p, &s);
    gsl_linalg_LU_solve(&gsl_a.matrix, p, &gsl_b.vector, gsl_x);

    for (int i = 0; i < size; i++)
        vector_x[i] = gsl_vector_get(gsl_x, i);
}

double measure_time(enum solution_type type, int size)
{
    clock_t start, end;
    double cpu_time_used = 0;

    for (int i = 0; i < TIME_TESTS; i++) 
    {
        start = clock();

        switch (type) {
            case SEQ: solve_seq(matrix_A, vector_x_seq, vector_b, size);
            break;

            case PAR: solve_par(matrix_A, vector_x_par, vector_b, size);
            break;

            case GSL: solve_gsl(matrix_A, vector_x_gsl, vector_b, size);
            break;
        }
        end = clock();

        cpu_time_used += ((double) (end - start)) / CLOCKS_PER_SEC;
    }

    return cpu_time_used / TIME_TESTS;
}

void generate_matrix(double *matrix, int n, int m)
{
    for (int i = 0; i < n; i++) 
        for (int j = 0; j < m; j++) 
            matrix[i * n + j] = (double) (rand() % MAX_RAND);
}

void generate_vector(double *vector, int n)
{
    for (int i = 0; i < n; i++)
        vector[i] = (double) (rand() % MAX_RAND);
}

int compare_vectors(double *vect1, double *vect2, int size) 
{
    for (int i = 0; i < size; i++)
        if (abs(vect1[i] - vect2[i]) > SOL_PRECISION)
            return 0;
    
    return 1;
}

int main(int argc, char* argv[])
{   
    if (argc != 2) 
    {
        printf("Wrong number of args.\n");
        exit(1);
    }

    int size = atoi(argv[1]);

    if (size < 0)
    {
        printf("Size can't be negative.\n");
        exit(1);
    }

    matrix_A = malloc(sizeof(double) * size * size);
    vector_x_seq = malloc(sizeof(double) * size);
    vector_x_par = malloc(sizeof(double) * size);
    vector_x_gsl = malloc(sizeof(double) * size);
    vector_b = malloc(sizeof(double) * size);

    if (matrix_A == NULL || vector_x_seq == NULL || vector_x_seq == NULL ||
        vector_x_gsl == NULL || vector_b == NULL)
    {
        printf("Cannot allocate memory for matrix or vectors.\n");
        exit(1);
    }

    srand(time(0));
    generate_matrix(matrix_A, size, size);
    generate_vector(vector_b, size);

    solve_seq(matrix_A, vector_x_seq, vector_b, size);
    solve_par(matrix_A, vector_x_par, vector_b, size);
    solve_gsl(matrix_A, vector_x_gsl, vector_b, size);

    if (compare_vectors(vector_x_seq, vector_x_par, size) && compare_vectors(vector_x_par, vector_x_gsl, size))
    {
        printf("All solutions are correct.\n");
    }
    else
    {
        printf("Mistake in some solution.\n"); 
        exit(1);
    }

    printf("Solutions were tested %d times.\n", TIME_TESTS);
    printf("Average time with size %d of sequential solution: %f\n", size, measure_time(SEQ, size));
    printf("Average time with size %d of parallel solution: %f\n", size, measure_time(PAR, size));
    printf("Average time with size %d of GSL solution: %f\n\n", size, measure_time(GSL, size));

    free(matrix_A);
    free(vector_x_seq);
    free(vector_x_par);
    free(vector_x_gsl);
    free(vector_b);

    return 0;
}