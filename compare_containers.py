import subprocess
import os

rhel_container = "dotnet/dotnet-20-rhel7"
other_containers = [
    "microsoft/dotnet:2.0-sdk-jessie",
    "microsoft/dotnet:2.0-sdk-stretch",
]
TIMING_THRESHOLD = .2 # percentage

# This will build and run the tests against a container and return the results
# in a form given by the process_test_results method.
def run_tests(container):
    cmd = "docker run" + \
        " --rm" + \
        " -u 0" + \
        " -v=" + os.path.dirname(os.path.realpath(__file__)) + "/tests:/opt/app-root/src:Z" + \
        " -it " + \
        container + \
        " bash -c \"cd /opt/app-root/src;dotnet clean;dotnet run\""
    # TODO: Add a try/catch and handle the error if it fails to run
    out = subprocess.check_output(cmd, stderr=subprocess.STDOUT, shell=True)
    return process_test_results(out.strip())

# This looks through the given string line by line a processes any line that
# looks like '{0} : {1}' and interprets that line as a timing test, with {0}
# being the test name and {1} being the test time in ms.
def process_test_results(results):
    if results is None:
        return None

    times = {}
    for line in results.splitlines():
        if line is None:
            continue
        parts = line.split(' : ', 1);
        if len(parts) is not 2:
            continue
        times[parts[0]] = parts[1].replace('ms', '')
    return times

# This compares the given dicts and compares the firsts timings to the seconds.
# If it is too much larger or smaller (defined by the TIMING_THRESHOLD constant)
# then it considers this a problem, prints a message to the console, and returns
# True.
def discrepancies_in_results(given_results, expected_results):
    problems_seen = False
    for item in given_results['timings']:
        if item not in expected_results['timings']:
            print "Problem: Test result {} found in {} was not found in {}.".format(
                item,
                given_results['name'],
                expected_results['name']
            )
            problems_seen = True
            continue

        found = int(given_results['timings'][item])
        expected = int(expected_results['timings'][item])
        # NOTE: Do we care if we the found results are 'too fast' compared to
        # what is expected?
        lower_desried = expected * (1 - TIMING_THRESHOLD)
        upper_desried = expected * (1 + TIMING_THRESHOLD)
        if (found < lower_desried or found > upper_desried):
            print "Problem: Test {} in {} had a time of {} ms but expected between {} and {}, according to {} and a threshold of {}".format(
                item,
                given_results['name'],
                found,
                lower_desried,
                upper_desried,
                expected_results['name'],
                TIMING_THRESHOLD
            )
            problems_seen = True
        del expected_results['timings'][item]

    # Any items left in 'timings' don't exist in given_results, which means
    # that they didn't run for some reason. We should treat that as a problem.
    for item in expected_results['timings']:
        print "Problem: Test result {} missing in {} was found in {}.".format(
            item,
            given_results['name'],
            expected_results['name']
        )
        problems_seen = True

    return problems_seen

rhel_results = {
    'name' : rhel_container,
    'timings' : run_tests(rhel_container)
}

other_results = []
for container in other_containers:
    other_results.append(
        {
            'name' : container,
            'timings' : run_tests(container)
        }
    )

exit_code = 0
for results in other_results:
    if discrepancies_in_results(rhel_results, results):
        exit_code = 1

exit(exit_code)
