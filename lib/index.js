const _package = require('../package.json')
const path = require('path')
const os = require('os')
const fs = require('fs')
const axios = require('axios')
const unzipper = require('unzipper')

const {execSync} = require('child_process')

async function downloadAndInstallNugetInspector() {
    const version = _package.version
    console.log('Checking if Nuget-Inspector ' + version + ' is installed and available')
    const currentVersionFolder = path.resolve(os.homedir(), '.dxw', 'depinder', 'nuget-inspector', _package.version)
    let platformName = getPlatformName();
    const nugetInspectorExePath = path.resolve(currentVersionFolder, platformName === 'win' ? 'NugetDotnet5Inspector.exe' : 'NugetDotnet5Inspector');

    if (!fs.existsSync(nugetInspectorExePath)) {
        fs.mkdirSync(currentVersionFolder, {recursive: true})
        console.log(`Downloading NugetInspector ${_package.version}`)
        const downloadedFile = await downloadFile(`https://github.com/dxworks/nuget-dotnet5-inspector/releases/download/v${_package.version}-depinder/nuget-inspector-${platformName}.zip`, path.resolve(currentVersionFolder, 'nuget-inspector.zip'))
        console.log(`Download Finished`)
        console.log('Installing...')
        await unzip(downloadedFile, {path: currentVersionFolder, overwriteRootDir: true})
        fs.rmSync(downloadedFile, {force: true})
        console.log('Install Finished')
        fs.chmodSync(nugetInspectorExePath, '755')
    } else {
        console.log('Found local installation')
    }
    return {currentVersionFolder, nugetInspectorExePath};
}

async function runNugetInspector(options) {
    const {currentVersionFolder, nugetInspectorExePath} = await downloadAndInstallNugetInspector();

    const args = options?.args? options.args : [...process.argv];

    await execSync(`${nugetInspectorExePath} ${args.join(' ')}`, {cwd: options?.workingDirectory? process.cwd(): currentVersionFolder, stdio: 'inherit'})
}

async function runNuGetInspectorProgrammatically(targetPath, outputDirectory, cwd) {
    const {currentVersionFolder, nugetInspectorExePath} = await downloadAndInstallNugetInspector();

    const args = [`-target_path=${targetPath}`, `-output_directory=${outputDirectory}`];

    return execSync(`${nugetInspectorExePath} ${args.join(' ')}`, {cwd: cwd ? cwd : currentVersionFolder})
}

function getPlatformName() {
    switch (process.platform) {
        case 'win32':
            return 'win'
        case 'darwin':
            return 'osx'
        case 'linux':
            return 'linux'
        default:
            throw Error('Nuget Inspector can only be installed on Windows, Mac or Linux systems')
    }
}

async function downloadFile(url, filename, payload, progressBar) {
    const file = fs.createWriteStream(filename, 'utf-8')
    let receivedBytes = 0

    const {data, headers, status} = await axios.get(url,
        {
            method: 'GET',
            responseType: 'stream',
        })

    const totalBytes = headers['content-length'] ? +headers['content-length'] : 0

    return new Promise((resolve, reject) => {
        if (status !== 200) {
            return reject('Response status was ' + status)
        }
        progressBar?.start(totalBytes, 0, payload)
        data
            .on('data', (chunk) => {
                receivedBytes += chunk.length
                progressBar?.update(receivedBytes, payload)
            })
            .pipe(file)
            .on('finish', () => {
                file.close()
                resolve(filename)
            })
            .on('error', (err) => {
                fs.unlinkSync(filename)
                progressBar?.stop()
                return reject(err)
            })
    })
}

async function unzip(zipFileName, options) {
    return new Promise((resolve, reject) => {
        if (options?.overwriteRootDir) {
            fs.createReadStream(zipFileName)
                .pipe(unzipper.Parse())
                .on('entry', function (entry) {
                    const fullPathName = path.resolve(options.path, entry.path.substring(entry.path.indexOf('/') + 1, entry.path.length))
                    if (entry.type === 'Directory') {
                        if (!fs.existsSync(fullPathName))
                            fs.mkdirSync(fullPathName, {recursive: true})
                    } else
                        entry.pipe(fs.createWriteStream(fullPathName))
                })
                .on('finish', () => {
                    resolve()
                })
                .on('error', reject)
        } else {
            fs.createReadStream(zipFileName)
                .pipe(unzipper.Extract(options))
                .on('finish', () => {
                    resolve()
                })
                .on('error', reject)
        }
    })
}

module.exports = {runNugetInspector, runNuGetInspectorProgrammatically}
