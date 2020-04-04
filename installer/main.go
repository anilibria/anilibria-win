package main

import (
	"archive/zip"
	"fmt"
	"io"
	"log"
	"os"
	"os/exec"
	"path/filepath"
	"strings"
	"io/ioutil"

	"golang.org/x/sys/windows/registry"
)

func main() {
	extractFiles()

	installCertificate()

	setSideloadingKeysInRegistry()

	runAppInstaller()
}

func extractFiles() {
	extractFileFromResources("tools/certificate.cer", "certificate.cer");

	extractFileFromResources("tools/certutil.exe", "certutil.exe");

	extractFileFromResources("tools/ic.bat", "ic.bat");
}

func extractFileFromResources(path string, resultFile string) {
	fileData, err := Asset(path)
	if err != nil {
		log.Fatal(err)
	}
	
	err = ioutil.WriteFile(resultFile, fileData, 0644)
	if err != nil {
		log.Fatal(err)
	}
}

func setSideloadingKeysInRegistry() {
	k, err := registry.OpenKey(registry.LOCAL_MACHINE, `\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock`, registry.QUERY_VALUE|registry.SET_VALUE)
	if err != nil {
		log.Fatal(err)
	}
	if err := k.SetDWordValue("AllowAllTrustedApps", 1); err != nil {
		log.Fatal(err)
	}
	if err := k.SetDWordValue("AllowDevelopmentWithoutDevLicense", 0); err != nil {
		log.Fatal(err)
	}
	if err := k.Close(); err != nil {
		log.Fatal(err)
	}
	defer k.Close()
}

func installCertificate() {
	fmt.Println("Installing certificate....")

	cmd := exec.Command(`ic.bat`)
	cmd.Stdout = os.Stdout
	cmd.Stdin = os.Stdin
	cmd.Stderr = os.Stderr

	err := cmd.Run()
	if err != nil && err.Error() != "exit status 1" {
		fmt.Println("Error: " + err.Error())
	}

	fmt.Println("Certificate installed")
}

func runAppInstaller() {
	cmd := exec.Command("ms-appinstaller:?source=https://anilibria.github.io/anilibria-win/dist/Anilibria.appinstaller")
	cmd.Start()
	cmd.Wait()
}