export interface RunNugetInspectorOptions {
    args?: string[];
    workingDirectory?: string;
}

export function runNugetInspector(options?: RunNugetInspectorOptions): Promise<void>;
export function runNuGetInspectorProgrammatically(targetPath: string, outputDirectory: string, cwd?: string): Promise<string>;
